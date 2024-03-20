using Grpc.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Userforchat;
using Users.Application.UseCases.Queries;
using static MassTransit.ValidationResultExtensions;

namespace Users.Infrastructure.Services.grpcServices
{
    public class grpcUserForChat_Service : UserForChatService.UserForChatServiceBase
    {
        private readonly IMediator _mediator;

        public grpcUserForChat_Service(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task<GetUserForChatResponse> GetUserForChat(GetUserForChatRequest request, ServerCallContext context)
        {
            var response = new GetUserForChatResponse();
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
                    var tempuser = new GiveUserForChat
                    {
                        UserId = "0",
                        NickName = "",
                        Photo = Google.Protobuf.ByteString.CopyFrom(new byte[] { })
                    };
                    response.Users.Add(tempuser);
                    return response;
                };

                foreach(var user in result)
                {
                    var tempuser = new GiveUserForChat
                    {
                        UserId = user.Id,
                        NickName = user.NickName,
                        Photo = Google.Protobuf.ByteString.CopyFrom(new byte[] { })
                    };
                    response.Users.Add(tempuser);
                }
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
