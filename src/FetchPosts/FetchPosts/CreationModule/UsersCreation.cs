using Dapper;
using FetchPosts.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchPosts.CreationModule
{
    public static class UsersCreation
    {
        public static async Task<bool> CreateUsers()
        {
            var users = new List<User>();
            string filePath = @"C:\Users\rostd\OneDrive\Desktop\Kyrsova_TryIt\src\FetchPosts\FetchPosts\Names.txt";
            string usersDb = "Data Source=ROSTIKPC\\SQLEXPRESS;Initial Catalog=Users;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
            string chatsDb = "Data Source=ROSTIKPC\\SQLEXPRESS;Initial Catalog=Chats;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
            string postsDb = "Data Source=ROSTIKPC\\SQLEXPRESS;Initial Catalog=Posts;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";

            foreach (var line in File.ReadAllLines(filePath))
            {
                var trimmed = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    users.Add(new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = trimmed,
                        NickName = trimmed
                    });
                }
            }

            foreach (var user in users)
            {
                await using var usersConn = new SqlConnection(usersDb);
                await usersConn.ExecuteAsync(
                @"INSERT INTO Users 
                  (Id, Name, NickName, Email, Phone, Bio, Photo, DateOfBirth, SexId, IsBanned, IsPrivate, IsCheckingMessages)
                  VALUES 
                  (@Id, @Name, @NickName, @Email, @Phone, @Bio, @Photo, @DateOfBirth, @SexId, @IsBanned, @IsPrivate, @IsCheckingMessages)",
                user);
                await using var chatsConn = new SqlConnection(chatsDb);
                await chatsConn.ExecuteAsync("INSERT INTO Users (Id, IsCheckingMessages) VALUES (@Id, 0)", new { user.Id });

                await using var postsConn = new SqlConnection(postsDb);
                await postsConn.ExecuteAsync("INSERT INTO Users (Id) VALUES (@Id)", new { user.Id });
            }

            return true;
        }


    }
}
