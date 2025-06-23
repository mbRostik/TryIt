using Dapper;
using FetchPosts.Models;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace FetchPosts.CreationModule
{
    public static class PostsCreation
    {
        public static async Task<bool> CreatePosts()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Console.WriteLine("Posts creation started...");
            string postsDb = "Data Source=ROSTIKPC\\SQLEXPRESS;Initial Catalog=Posts;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
            string filePath = @"C:\Users\rostd\OneDrive\Desktop\Kyrsova_TryIt\src\FetchPosts\FetchPosts\posts.xlsx";

            List<string> userIds = new List<string>();
            await using var postConn = new SqlConnection(postsDb);
            userIds = (await postConn.QueryAsync<string>("SELECT Id FROM Users")).AsList();

            var posts = new List<Post>();



            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 1; row <= rowCount; row++)
                {
                    var title = worksheet.Cells[row, 1].Text;
                    var content = worksheet.Cells[row, 2].Text;

                    posts.Add(new Post { Title = title, Content = content });
                }
            }

            int userIndex = 0;

            foreach (var post in posts)
            {
                if (userIndex == 500)
                {
                    userIndex = 0;
                }
                post.UserId = userIds[userIndex];
                post.Date = DateTime.Now;

                await postConn.ExecuteAsync(
                    "INSERT INTO Posts (Title, Content, UserId, Date) VALUES (@Title, @Content, @UserId, @Date)",
                    post
                );
                userIndex++;
            }

            return true;
        }

        public static async Task<bool> AddPictures()
        {
            string postsDb = "Data Source=ROSTIKPC\\SQLEXPRESS;Initial Catalog=Posts;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
            string imagesRoot = @"D:\C\Projects\AI_TryIt\Pictures_Categories\Data\imagenet_dataset";

            List<int> postIds = new List<int>();
            await using var postConn = new SqlConnection(postsDb);
            postIds = (await postConn.QueryAsync<int>(
                "SELECT Id FROM Posts ORDER BY Id"
            )).ToList();

            string[] folders = Directory.GetDirectories(imagesRoot);

            using var httpClient = new HttpClient();
            int postCounter = 0;
            foreach (var postId in postIds)
            {
                postCounter++;

                List<string> selectedCategories;

                if (postCounter <= 1000)
                    selectedCategories = new List<string> { "coffeepot", "pajama" };
                else if (postCounter <= 2000)
                    selectedCategories = new List<string> { "church", "dock" };
                else if (postCounter <= 3000)
                    selectedCategories = new List<string> { "mask", "suit" };
                else if (postCounter <= 4000)
                    selectedCategories = new List<string> { "bicycle_built_for_two", "canoe" };
                else if (postCounter <= 5000)
                    selectedCategories = new List<string> { "notebook", "organ" };
                else if (postCounter <= 6000)
                    selectedCategories = new List<string> { "car_wheel", "limousine" };
                else if (postCounter <= 7000)
                    selectedCategories = new List<string> { "monastery", "palace" };
                else if (postCounter <= 8000)
                    selectedCategories = new List<string> { "cliff_dwelling", "mountain" };
                else if (postCounter <= 9000)
                    selectedCategories = new List<string> { "mountain", "horse" };
                else if (postCounter <= 10000)
                    selectedCategories = new List<string> { "koala", "horse" };
                else if (postCounter <= 11000)
                    selectedCategories = new List<string> { "mushroom", "strawberry" };
                else if (postCounter <= 12000)
                    selectedCategories = new List<string> { "stage", "harmonica" };
                else if (postCounter <= 13000)
                    selectedCategories = new List<string> { "cinema", "stage" };
                else if (postCounter <= 14000)
                    selectedCategories = new List<string> { "church", "palace" };
                else
                    selectedCategories = new List<string>();

                var selectedFolders = selectedCategories
                    .Select(name => Path.Combine(imagesRoot, name))
                    .Where(Directory.Exists)
                    .ToList();
                var imagePaths = new List<string>();

                foreach (var folder in selectedFolders)
                {
                    var images = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                                          .Where(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".JPEG"))
                                          .OrderBy(_ => Guid.NewGuid())
                                          .Take(new Random().Next(1, 3));

                    imagePaths.AddRange(images);
                }

                if (imagePaths.Count == 0)
                    continue;

                var form = new MultipartFormDataContent();
                foreach (var imagePath in imagePaths)
                {
                    var imageContent = new StreamContent(File.OpenRead(imagePath));
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    form.Add(imageContent, "files", Path.GetFileName(imagePath));
                }

                try
                {
                    var response = await httpClient.PostAsync("http://127.0.0.1:8000/predict-images", form);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed for PostId {postId}: {response.StatusCode}");
                        continue;
                    }
                    var responseJson = await response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>();
                    var labels = responseJson?["labels"] ?? new List<string>();

                    var existingCategories = (await postConn.QueryAsync<string>(
                        "SELECT CategoryName FROM PostPhotoCategories"
                    )).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var newLabels = labels.Where(label => !existingCategories.Contains(label)).Distinct().ToList();

                    foreach (var newLabel in newLabels)
                    {
                        await postConn.ExecuteAsync(
                            "INSERT INTO PostPhotoCategories (CategoryName) VALUES (@CategoryName)",
                            new { CategoryName = newLabel }
                        );
                        existingCategories.Add(newLabel);
                    }

                    var postFiles = new List<PostFile>();

                    foreach (var imagePath in imagePaths)
                    {
                        var fileBytes = await File.ReadAllBytesAsync(imagePath);

                        postFiles.Add(new PostFile
                        {
                            Name = Path.GetFileName(imagePath),
                            file = fileBytes,
                            Date = DateTime.Now,
                            PostId = postId
                        });
                    }

                    foreach (var pf in postFiles)
                    {
                        await postConn.ExecuteAsync(
                            "INSERT INTO PFiles (Name, [file], Date, PostId) VALUES (@Name, @file, @Date, @PostId)",
                            pf
                        );
                    }

                    var labelIdMap = (await postConn.QueryAsync<(int Id, string CategoryName)>(
                        "SELECT Id, CategoryName FROM PostPhotoCategories"
                    )).ToDictionary(x => x.CategoryName, x => x.Id, StringComparer.OrdinalIgnoreCase);

                    var postCategoryPairs = labels
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .Where(label => labelIdMap.ContainsKey(label))
                        .Select(label => new
                        {
                            PostId = postId,
                            PostPhotoCategoryId = labelIdMap[label]
                        });

                    foreach (var pair in postCategoryPairs)
                    {
                        await postConn.ExecuteAsync(
                            "INSERT INTO PostsWithPhotoCategories (PostId, PostPhotoCategoryId) VALUES (@PostId, @PostPhotoCategoryId)",
                            pair
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error for PostId {postId}: {ex.Message}");
                }
            }

            return true;

        }

        public static async Task<bool> AddLikes2()
        {
            string postsDb = "Data Source=ROSTIKPC\\SQLEXPRESS;Initial Catalog=Posts;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
            List<string> userIds = new List<string>();
            List<PostCategoryStats> statsList = new List<PostCategoryStats>();

            await using var postConn = new SqlConnection(postsDb);
            userIds = (await postConn.QueryAsync<string>("SELECT Id FROM Users")).AsList();

            string statsQuery = @"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY COUNT(DISTINCT p.Id) DESC) AS Id,
                    twc.PostTextCategoryId,
                    pwc.PostPhotoCategoryId,
                    COUNT(DISTINCT p.Id) AS PostCount,
                    STRING_AGG(CAST(p.Id AS VARCHAR), ',') AS PostIds
                FROM Posts.dbo.Posts p
                INNER JOIN Posts.dbo.PostsWithTextCategories twc ON p.Id = twc.PostId
                INNER JOIN Posts.dbo.PostsWithPhotoCategories pwc ON p.Id = pwc.PostId
                GROUP BY 
                    twc.PostTextCategoryId, 
                    pwc.PostPhotoCategoryId
                HAVING COUNT(DISTINCT p.Id) >= 80
                ORDER BY PostCount DESC;";

            statsList = (await postConn.QueryAsync<PostCategoryStats>(statsQuery)).AsList();
            var random = new Random();
            foreach (var userId in userIds)
            {
                var selectedGroup = statsList[random.Next(statsList.Count)];

                var postIdList = selectedGroup.PostIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .OrderBy(_ => random.Next())
                .Take((int)(selectedGroup.PostCount * 0.7))
                .ToList();

                const string insertQuery = @"
                INSERT INTO [Posts].[dbo].[PostReactionts] (UserId, PostId, Reaction, Date)
                VALUES (@UserId, @PostId, @Reaction, @Date);";

                foreach (var postId in postIdList)
                {
                    var reaction = new PostReaction
                    {
                        UserId = userId,
                        PostId = postId,
                        Reaction = true,
                        Date = DateTime.UtcNow
                    };

                    await postConn.ExecuteAsync(insertQuery, reaction);
                }
            }

            return true;
        }


        public static async Task<bool> AddBalancedLikes()
        {
            string postsDb = "Data Source=ROSTIKPC\\SQLEXPRESS;Initial Catalog=Posts;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
            List<string> userIds = new List<string>();
            List<PostCategoryStats> statsList = new List<PostCategoryStats>();

            await using var postConn = new SqlConnection(postsDb);
            userIds = (await postConn.QueryAsync<string>("SELECT Id FROM Users")).AsList();

            string statsQuery = @"
        SELECT 
            ROW_NUMBER() OVER (ORDER BY COUNT(DISTINCT p.Id) DESC) AS Id,
            twc.PostTextCategoryId,
            pwc.PostPhotoCategoryId,
            COUNT(DISTINCT p.Id) AS PostCount,
            STRING_AGG(CAST(p.Id AS VARCHAR), ',') AS PostIds
        FROM Posts.dbo.Posts p
        INNER JOIN Posts.dbo.PostsWithTextCategories twc ON p.Id = twc.PostId
        INNER JOIN Posts.dbo.PostsWithPhotoCategories pwc ON p.Id = pwc.PostId
        GROUP BY 
            twc.PostTextCategoryId, 
            pwc.PostPhotoCategoryId
        HAVING COUNT(DISTINCT p.Id) >= 30
        ORDER BY PostCount DESC;";

            statsList = (await postConn.QueryAsync<PostCategoryStats>(statsQuery)).AsList();
            var random = new Random();

            const int likesPerUser = 100;
            const int dislikesPerUser = 50;

            foreach (var userId in userIds)
            {
                var selectedPositiveGroups = statsList
                    .OrderBy(_ => random.Next())
                    .Take(3)
                    .ToList();

                var selectedNegativeGroups = statsList
                    .Where(x => !selectedPositiveGroups.Contains(x))
                    .OrderBy(_ => random.Next())
                    .Take(2)
                    .ToList();

                var likePostIds = selectedPositiveGroups
                    .SelectMany(g => g.PostIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .OrderBy(_ => random.Next())
                        .Take(likesPerUser / selectedPositiveGroups.Count))
                    .ToList();

                var dislikePostIds = selectedNegativeGroups
                    .SelectMany(g => g.PostIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .OrderBy(_ => random.Next())
                        .Take(dislikesPerUser / selectedNegativeGroups.Count))
                    .ToList();

                const string insertQuery = @"
            INSERT INTO [Posts].[dbo].[PostReactionts] (UserId, PostId, Reaction, Date)
            VALUES (@UserId, @PostId, @Reaction, @Date);";

                foreach (var postId in likePostIds)
                {
                    var reaction = new PostReaction
                    {
                        UserId = userId,
                        PostId = postId,
                        Reaction = true,
                        Date = DateTime.UtcNow
                    };

                    await postConn.ExecuteAsync(insertQuery, reaction);
                }

                foreach (var postId in dislikePostIds)
                {
                    var reaction = new PostReaction
                    {
                        UserId = userId,
                        PostId = postId,
                        Reaction = false,
                        Date = DateTime.UtcNow
                    };

                    await postConn.ExecuteAsync(insertQuery, reaction);
                }
            }

            return true;


        }
        private class PostCategoryStats
        {
            public int Id { get; set; }
            public int PostTextCategoryId { get; set; }
            public int PostPhotoCategoryId { get; set; }
            public int PostCount { get; set; }
            public string PostIds { get; set; }
        }
    }
}