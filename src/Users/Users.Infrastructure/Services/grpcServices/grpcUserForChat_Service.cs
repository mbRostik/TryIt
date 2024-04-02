using AutoMapper;
using Grpc.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Userforchat;
using Users.Application.Contracts.DTOs;
using Users.Application.Contracts.Interfaces;
using Users.Application.UseCases.Queries;
using static MassTransit.ValidationResultExtensions;

namespace Users.Infrastructure.Services.grpcServices
{
    public class grpcUserForChat_Service : UserForChatService.UserForChatServiceBase
    {
        private readonly IMediator _mediator;
        private readonly IMapperService _mapper;
        public readonly Serilog.ILogger _logger;

        public grpcUserForChat_Service(IMediator mediator, IMapperService mapperService, Serilog.ILogger logger)
        {
            _mediator = mediator;
            _mapper = mapperService;
            _logger = logger;
        }
        public override async Task<GetUserForChatResponse> GetUserForChat(GetUserForChatRequest request, ServerCallContext context)
        {
            _logger.Information($"Starting GetUserForChat for UserIds: [{string.Join(", ", request.UserId)}]");

            var response = new GetUserForChatResponse();
            var mapper = _mapper.Mapper_UserChatProfileToGiveUserForChat();
            try
            {
                List<string> userIds = new List<string>();

                foreach (var user in request.UserId)
                {
                    userIds.Add(user);
                }

                var result = await _mediator.Send(new GetListUsersQuery(userIds));

                if (result == null)
                {
                    _logger.Warning("No users found for given UserIds");

                    var tempuser = new GiveUserForChat
                    {
                        UserId = "0",
                        NickName = "",
                        Photo = Google.Protobuf.ByteString.CopyFrom(new byte[] { })
                    };
                    response.Users.Add(tempuser);
                    return response;
                };
                _logger.Information($"Found {result.Count} users for given UserIds");

                foreach (var user in result)
                {
                    GiveUserForChat tempuser = mapper.Map<GiveUserForChat>(user);

                    response.Users.Add(tempuser);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while processing GetUserForChat for UserIds: [{string.Join(", ", request.UserId)}]");
                var tempuser = new GiveUserForChat
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
