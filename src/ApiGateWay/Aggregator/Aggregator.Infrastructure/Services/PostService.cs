using Aggregator.Application.Contracts.DTOs;
using Aggregator.Application.Contracts.Interfaces;
using Aggregator.Infrastructure.Services.ProtoServices;
using Aggregator.WebApi.Services.ProtoServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator.Infrastructure.Services
{
    public class PostService : IPostService
    {
        private readonly Serilog.ILogger logger;
        private GrpcGetUserForPostsService _userGrpcService { get; set; }
        private GrpcGetUserPostsService _postGrpcService { get; set; }

        public PostService(GrpcGetUserPostsService postgrpcService, GrpcGetUserForPostsService usergrpcService, Serilog.ILogger logger)
        {
            this._postGrpcService = postgrpcService;
            this._userGrpcService = usergrpcService;
            this.logger = logger;
        }

        public async Task<List<GiveFollowedPostsDTO>> GetFollowedPosts(string userId, string accessToken)
        {
            logger.Information("Starting to retrieve Follows for UserId: {UserId}", userId);

            try
            {
                var follows = await _userGrpcService.GetUserForPostAsync(accessToken, userId);

                if ((follows.Users.Count == 1 && follows.Users[0].UserId == "0" && follows.Users[0].NickName == "") || !follows.Users.Any())
                {
                    logger.Information("No follows found for UserId: {UserId}", userId);
                    return null;
                }


                var tasks = follows.Users.Select(async user =>
                {
                    var postsResponse = await _postGrpcService.GetUserForPostAsync(user.UserId, accessToken);
                    if (!postsResponse.Posts.Any())
                    {
                        return null;
                    }
                    return postsResponse.Posts.Select(post => new GiveFollowedPostsDTO
                    {
                        NickName = user.NickName,
                        UserId = user.UserId,
                        Photo = user.Photo.ToByteArray(),
                        Title = post.Title,
                        Content = post.Content,
                        Id = post.Id,
                        PostFiles = post.Files.Select(file => new PostFile
                        {
                            File = file.File.ToByteArray(),
                            Name = file.Name,
                            PostId = post.Id
                        }).ToList()
                    }).ToList();
                });

                var postsLists = await Task.WhenAll(tasks);
                var result = postsLists
                    .Where(posts => posts != null)
                    .SelectMany(posts => posts)
                    .ToList();
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error retrieving user chats for UserId: {UserId}", userId);
                return new List<GiveFollowedPostsDTO>();
            }
        }
    }
}
