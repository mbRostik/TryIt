using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Posts.Application.Contracts.Interfaces;
using Posts.Application.UseCases.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Userposts;
namespace Posts.Infrastructure.Services.grpcServices
{
    internal class grpcUserPosts_Service : UserPostsService.UserPostsServiceBase
    {
        private readonly IMediator _mediator;

        public grpcUserPosts_Service(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task<GetUserPostsResponse> GetUserPosts(GetUserPostsRequest request, ServerCallContext context)
        {
            var response = new GetUserPostsResponse();
            try
            {

                var result = await _mediator.Send(new GetUserPostsQuery(request.UserId));

                if (result == null)
                {
                    var temppost = new GiveUserPosts
                    {
                        Id = 0,
                        Title = "",
                        Content = "",
                        Date = null
                    };
                    temppost.Files.Add(new PFile());
                    response.Posts.Add(temppost);
                    return response;
                };

                foreach (var post in result)
                {
                    GiveUserPosts tempuser = new GiveUserPosts
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Content = post.Content,
                        Date = Timestamp.FromDateTime(post.Date.ToUniversalTime())
                    };

                    foreach(var file in post.Files)
                    {
                        PFile tempFile = new PFile
                        {
                            Name = file.Name,
                            PostId = file.PostId,
                            File = Google.Protobuf.ByteString.CopyFrom(file.file)
                        };
                        tempuser.Files.Add(tempFile);
                    }

                    response.Posts.Add(tempuser);
                }
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                var temppost = new GiveUserPosts
                {
                    Id = 0,
                    Title = "",
                    Content = "",
                    Date = null
                };
                temppost.Files.Add(new PFile());
                response.Posts.Add(temppost);
                return response;
            }
        }
    }
}
