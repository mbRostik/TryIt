using Chats.Application.UseCases.Queries;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Userchats;

namespace Chats.Infrastructure.Services.grpcServices
{
    public class grpcUserChats_Service : UserChatsService.UserChatsServiceBase
    {
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger logger;

        public grpcUserChats_Service(IMediator mediator, Serilog.ILogger logger)
        {
            _mediator = mediator;
            this.logger= logger;
        }
        public override async Task<GetUserChatsResponse> GetUserChats(GetUserChatsRequest request, ServerCallContext context)
        {
            var response = new GetUserChatsResponse();

            logger.Information($"Processing GetUserChats for UserId: {request.UserId}");

            try
            {
                var result = await _mediator.Send(new GetUserChatsQuery(request.UserId));
                if (result == null)
                {
                    logger.Warning($"GetUserChats returned null for UserId: {request.UserId}");
                    var chat = new GiveUserChats
                    {
                        Chatid = 0,
                        ContactId = "",
                        LastActivity = null,
                        LastMessage = ""
                    };
                    response.Chats.Add(chat);
                    return response;
                }
                foreach (var item in result)
                {
                    if (item.LastMessage != null && item.LastMessageSenderId != null && item.LastActivity != null)
                    {
                        Timestamp lastActivityTimestamp = null;
                        if (item.LastActivity.HasValue)
                        {
                            lastActivityTimestamp = Timestamp.FromDateTime(item.LastActivity.Value.ToUniversalTime());
                        }
                        var chat = new GiveUserChats
                        {
                            Chatid = item.ChatId,
                            ContactId = item.ContactId,
                            LastActivity = lastActivityTimestamp,
                            LastMessage = item.LastMessage,
                            LastMessageSenderId = item.LastMessageSenderId
                        };
                        response.Chats.Add(chat);
                    }
                }

                logger.Information($"Successfully processed GetUserChats for UserId: {request.UserId}");
                return response;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error processing GetUserChats for UserId: {request.UserId}");
                var chat = new GiveUserChats
                {
                    Chatid = 0,
                    ContactId = "",
                    LastActivity = null,
                    LastMessage = ""
                };
                response.Chats.Add(chat);
                return response;
            }
        }
    }
}