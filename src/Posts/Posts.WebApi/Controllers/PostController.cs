using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Posts.Application.Contracts.DTOs;
using Posts.Application.Contracts.Validators;
using Posts.Application.UseCases.Commands;
using Posts.Application.UseCases.Handlers.OperationHandlers;
using Posts.Application.UseCases.Queries;
using Posts.Domain.Entities;
using Posts.Infrastructure.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Posts.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger logger;
        private readonly PostDbContext dbContext;
        public PostController(IMediator mediator, Serilog.ILogger logger, PostDbContext postDbContext)
        {
            this.mediator = mediator;
            this.dbContext = postDbContext;
            this.logger = logger;
        }

        [HttpGet("GetUserPosts")]
        public async Task<ActionResult<List<GiveProfilePostsDTO>>> GetUserPosts()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            logger.Information("Starting GetUserPosts for UserId {UserId}", userId);

            try
            {
                var result = await mediator.Send(new GetUserPostsQuery(userId));
                logger.Information("Successfully fetched posts for UserId {UserId}", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error fetching posts for UserId {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("CreatePost")]
        public async Task<ActionResult> CreatePost([FromBody] CreatePostDTO model)
        {
            var validator = new CreatePostDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { error = e.ErrorMessage }));
            }

            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            logger.Information("Starting CreatePost for UserId {UserId}", userId);

            model.UserId = userId;

            try
            {
                var result = await mediator.Send(new CreatePostCommand(model));
                if (result)
                {
                    logger.Information("Post created successfully for UserId {UserId}", userId);
                    return Ok();
                }
                logger.Warning("Failed to create post for UserId {UserId}", userId);
                return BadRequest("There was a problem while creating the post");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error creating post for UserId {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("GetsmbPosts")]
        public async Task<ActionResult<List<GiveProfilePostsDTO>>> GetsmbPosts([FromBody] GetSmbPosts model)
        {
            logger.Information("Starting GetsmbPosts for ProfileId {ProfileId}", model.ProfileId);

            try
            {
                var result = await mediator.Send(new GetsmbPostsQuery(model.ProfileId));
                if (result.Any())
                {
                    logger.Information("Successfully fetched smbPosts for ProfileId {ProfileId}", model.ProfileId);
                    return Ok(result);
                }
                else
                {
                    logger.Warning("No posts found for ProfileId {ProfileId}", model.ProfileId);
                    return Ok(null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error fetching smbPosts for ProfileId {ProfileId}", model.ProfileId);
                return StatusCode(500, "Internal server error");
            }
        } 
        [HttpPost("DeleteUserPost")]
        public async Task<ActionResult> DeleteUserPost([FromBody] DeletePostDTO model)
        {
            logger.Information("Starting DeleteUserPost for Post {ProfileId}", model.PostId);

            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var result = await mediator.Send(new DeletePostCommand(model.PostId, userId));
                if (!result)
                {
                    logger.Warning("Smth went wrong for the Post {ProfileId}", model.PostId);
                    return BadRequest();
                }
                logger.Information("Post was deleted");

                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error DeleteUserPost for Post {ProfileId}", model.PostId);
                return StatusCode(500, "Internal server error");
            }
        }

        public class RecommendationInputModel
        {
            public int Skip { get; set; }
            public int Limit { get; set; }
            public List<int> ExcludeIds { get; set; }
        }

        [HttpPost("getrecommendedposts")]
        public async Task<ActionResult<List<GiveReccomendedPostDTO>>> GetRecommendedPosts([FromBody] RecommendationInputModel model)
        {
            var skip = model?.Skip ?? 0;
            var limit = model?.Limit ?? 30;
            var excludeIds = model?.ExcludeIds ?? new List<int>();
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    logger.Warning("Unauthorized request to GetRecommendedPosts");
                    return Unauthorized();
                }

                logger.Information("Requesting recommendations for user {UserId}", userId);

                var requestData = new
                {
                    user_id = userId,
                    exclude_post_ids = excludeIds?.Select(id => id.ToString()).ToList() ?? new List<string>(),
                    skip = skip,
                    limit = limit
                };

                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsync("http://localhost:8001/recommend-posts",
                    new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    logger.Error("Failed to fetch recommended posts for user {UserId} from GNN API. Status: {StatusCode}", userId, response.StatusCode);
                    return StatusCode(500, "Recommendation service error");
                }

                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonDocument.Parse(json);
                var postIds = parsed.RootElement.GetProperty("posts")
                    .EnumerateArray()
                    .Select(x => int.TryParse(x.GetString(), out var id) ? id : (int?)null)
                    .Where(id => id != null)
                    .Select(id => id.Value)
                    .ToList();

                if (!postIds.Any())
                {
                    logger.Warning("No post recommendations received for user {UserId}", userId);
                    return Ok(new List<GiveReccomendedPostDTO>());
                }

                logger.Information("Received {Count} recommended post IDs", postIds.Count);

                // Fetch posts with related data
                var posts = await dbContext.Posts
                    .AsNoTracking()
                    .Where(p => postIds.Contains(p.Id))
                    .Include(p => p.PostWithTextCategories)
                        .ThenInclude(ptc => ptc.PostTextCategory)
                    .Include(p => p.PostWithPhotoCategories)
                        .ThenInclude(ppc => ppc.PostPhotoCategory)
                    .Include(p => p.Files)
                    .ToListAsync();
                var userReactions = await dbContext.PostReactionts
                    .Where(r => r.UserId == userId && postIds.Contains(r.PostId))
                    .ToDictionaryAsync(r => r.PostId, r => (bool?)r.Reaction);

                var orderedPosts = postIds
                    .Select(id => posts.FirstOrDefault(p => p.Id == id))
                    .Where(p => p != null)
                    .Select(p => new GiveReccomendedPostDTO
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        Title = p.Title,
                        Content = p.Content,
                        Date = p.Date,
                        PostReaction = userReactions.TryGetValue(p.Id, out var reaction) ? reaction : null,
                        Files = p.Files?.Select(f => new GiveFileDTO
                        {
                            Id = f.Id,
                            Name = f.Name,
                            file = f.file,
                            Date = f.Date,
                            PostId = f.PostId
                        }).ToList() ?? new List<GiveFileDTO>(),
                        Tags = (p.PostWithTextCategories ?? Enumerable.Empty<PostWithTextCategories>())
                            .Where(ptc => ptc?.PostTextCategory != null)
                            .Select(ptc => ptc.PostTextCategory.CategoryName)
                            .Concat(
                                (p.PostWithPhotoCategories ?? Enumerable.Empty<PostWithPhotoCategories>())
                                .Where(ppc => ppc?.PostPhotoCategory != null)
                                .Select(ppc => ppc.PostPhotoCategory.CategoryName)
                            )
                            .Where(name => !string.IsNullOrWhiteSpace(name))
                            .Distinct()
                            .ToList()
                    })
                    .ToList();

                foreach (var post in orderedPosts)
                {
                    post.Files = post.Files.OrderBy(f => f.Date).ToList();
                }

                using HttpClient httpClient2 = new HttpClient();
                httpClient2.BaseAddress = new Uri("https://localhost:7075/User");

                foreach (var post in orderedPosts)
                {

                    var requestDto = new SomeonesProfileDTO
                    {
                        ProfileId = post.UserId
                    };

                    try
                    {
                        var responseE = await httpClient2.PostAsJsonAsync("User/GetSomeonesProfile2", requestDto);
                        if (responseE.IsSuccessStatusCode)
                        {
                            var profile = await responseE.Content.ReadFromJsonAsync<GiveSmbProfileDTO>();

                            if (profile != null)
                            {
                                post.Photo = profile.Photo ?? Array.Empty<byte>();
                                post.nickName = profile.NickName ?? profile.Name ?? "Unknown User";
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Request failed for {requestDto.ProfileId}: {responseE.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching profile for {requestDto.ProfileId}: {ex.Message}");
                    }

                }
                return Ok(orderedPosts);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error occurred while processing GetRecommendedPosts");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("react")]
        public async Task<IActionResult> ReactToPost([FromBody] PostReactionDTO dto)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.Warning("Unauthorized request to GetRecommendedPosts");
                return Unauthorized();
            }
            var existing = await dbContext.PostReactionts
                .FirstOrDefaultAsync(r => r.UserId == userId && r.PostId == dto.PostId);

            if (dto.IsLike == null)
            {
                if (existing != null)
                {
                    dbContext.PostReactionts.Remove(existing);
                    await dbContext.SaveChangesAsync();
                }
                return Ok();
            }

            if (existing != null)
            {
                if (existing.Reaction == dto.IsLike)
                {
                    return Ok();
                }

                existing.Reaction = dto.IsLike.Value;
                existing.Date = DateTime.UtcNow;
                dbContext.PostReactionts.Update(existing);
            }
            else
            {
                var newReaction = new PostReaction
                {
                    UserId = userId,
                    PostId = dto.PostId,
                    Reaction = dto.IsLike.Value,
                    Date = DateTime.UtcNow
                };
                await dbContext.PostReactionts.AddAsync(newReaction);
            }

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        private class SomeonesProfileDTO
        {
            public string ProfileId { get; set; }
        }
        private class GiveSmbProfileDTO
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";

            public string NickName { get; set; } = "";

            public string Email { get; set; } = "";

            public string Phone { get; set; } = "";

            public string Bio { get; set; } = "";

            public byte[] Photo { get; set; } = [];

            public DateTime DateOfBirth { get; set; } = DateTime.Now;
            public bool IsPrivate { get; set; } = false;

            public int FollowersCount { get; set; } = 0;

            public int FollowsCount { get; set; } = 0;

            public string SexId { get; set; } = "UnIdentify";

            public bool isFollowedByUser { get; set; } = false;
        }
    }
}
