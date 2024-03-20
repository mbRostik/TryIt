using Chats.Application.UseCases.Queries;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
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

        public grpcUserChats_Service(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task<GetUserChatsResponse> GetUserChats(GetUserChatsRequest request, ServerCallContext context)
        {
            var response = new GetUserChatsResponse();

            try
            {

                var result = await _mediator.Send(new GetUserChatsQuery(request.UserId));
                if (result == null)
                {
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
                        LastMessage = item.LastMessage
                    };
                    response.Chats.Add(chat);
                }

                return response;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
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