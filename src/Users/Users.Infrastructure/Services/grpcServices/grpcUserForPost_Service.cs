using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.Interfaces;
using Userforpost;
using Grpc.Core;
using Users.Application.UseCases.Queries;

namespace Users.Infrastructure.Services.grpcServices
{
    public class grpcUserForPost_Service : UserForPostService.UserForPostServiceBase
    {
        private readonly IMediator _mediator;
        public readonly Serilog.ILogger _logger;

        public grpcUserForPost_Service(IMediator mediator, Serilog.ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        public override async Task<GetUserForPostResponse> GetUserForPost(GetUserForPostRequest request, ServerCallContext context)
        {
            _logger.Information($"Starting GetUserForPost with UserId: {request.UserId}");

            var response = new GetUserForPostResponse();
            try
            {
                var follows = await _mediator.Send(new GetListOfFollowsQuery(request.UserId));

                if (follows == null)
                {
                    _logger.Warning($"No followers found for UserId: {request.UserId}");

                    var tempuser = new GiveUserForPost
                    {
                        UserId = "0",
                        NickName = "",
                        Photo = Google.Protobuf.ByteString.CopyFrom(new byte[] { })
                    };
                    response.Users.Add(tempuser);
                    return response;
                };

                _logger.Information($"Found {follows.Count} followers for UserId: {request.UserId}");

                foreach (var follow in follows)
                {
                    var user = await _mediator.Send(new GetUserByIdQuery(follow.UserId));
                    GiveUserForPost temp = new GiveUserForPost
                    {
                        UserId = user.Id,
                        NickName = user.NickName,
                        Photo = Google.Protobuf.ByteString.CopyFrom(user.Photo)
                    };
                    response.Users.Add(temp);
                }
                _logger.Information($"Successfully processed GetUserForPost for UserId: {request.UserId}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception thrown in GetUserForPost for UserId: {request.UserId}");
                var tempuser = new GiveUserForPost
                {
                    UserId = "0",
                    NickName = "",
                    Photo = Google.Protobuf.ByteString.CopyFrom(new byte[] { })
                };
                response.Users.Add(tempuser);
                return response;
            }
        }
    }
}