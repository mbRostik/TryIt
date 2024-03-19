using Grpc.Core;
using Grpc.Net.Client;
using Userchats;

namespace Aggregator.WebApi.Services.ProtoServices
{
    public class grpcGetUserChatsService
    {
        public async Task GetUserChatsAsync(string userId, string token)
        {
            var headers = new Metadata
                {
                    { "Authorization", $"Bearer {token}" }
                };

            using var channel = GrpcChannel.ForAddress("https://localhost:7234");

            var client = new UserChatsService.UserChatsServiceClient(channel);

            var request = new GetUserChatsRequest { UserId = userId };

            var response = await client.GetUserChatsAsync(request);

            foreach (var chat in response.Chats)
            {
                Console.WriteLine($"Chat ID: {chat.Chatid}, Contact ID: {chat.ContactId}, Last Message: {chat.LastMessage}");
            }
        }
    }
}
