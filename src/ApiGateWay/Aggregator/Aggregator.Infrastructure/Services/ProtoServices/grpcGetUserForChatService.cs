using Grpc.Core;
using Grpc.Net.Client;
using Userchats;
using Userforchat;

namespace Aggregator.WebApi.Services.ProtoServices
{
    public class grpcGetUserForChatService
    {
        public async Task<GetUserForChatResponse> GetUserChatsAsync(List<string> userIds, string token)
        {
            var headers = new Metadata
    {
        { "Authorization", $"Bearer {token}" }
    };

            using var channel = GrpcChannel.ForAddress("https://localhost:7075");
            var client = new UserForChatService.UserForChatServiceClient(channel);

            var request = new GetUserForChatRequest();

            request.UserId.AddRange(userIds);

            var response = await client.GetUserForChatAsync(request, headers);
            return response;
        }
    }
}
