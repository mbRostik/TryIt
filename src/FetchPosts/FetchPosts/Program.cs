using Dapper;
using FetchPosts.CreationModule;
using FetchPosts.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace UserSeeder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await UsersCreation.CreateUsers();
            //await PostsCreation.CreatePosts();
            //await PostsCreation.AddPictures();
            await PostsCreation.AddBalancedLikes();
            Console.WriteLine("✅ Users inserted into all databases.");
        }

    }
}