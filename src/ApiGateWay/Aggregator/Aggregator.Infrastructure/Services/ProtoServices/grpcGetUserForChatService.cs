using Aggregator.Infrastructure.Policies;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Userchats;
using Userforchat;

namespace Aggregator.WebApi.Services.ProtoServices
{
    public class GrpcGetUserForChatService
    {
        private readonly IConfiguration _configuration;
        private readonly GrpcPolly _grpcPolly;

        public GrpcGetUserForChatService(IConfiguration configuration, GrpcPolly grpcPolly)
        {
            _configuration = configuration;
            _grpcPolly = grpcPolly;
        }

        public async Task<GetUserForChatResponse> GetUserChatsAsync(List<string> userIds, string token)
        {
            var headers = new Metadata
            {
                { "Authorization", $"Bearer {token}" }
            };
            var serviceAddress = _configuration["GrpcServices:UserService:Address"];

            return await _grpcPolly.ExponentialGrpcRetry.ExecuteAsync(async () =>
            {
                using var channel = GrpcChannel.ForAddress(serviceAddress);
                var client = new UserForChatService.UserForChatServiceClient(channel);

                var request = new GetUserForChatRequest();
                request.UserId.AddRange(userIds);

                return await client.GetUserForChatAsync(request, headers);
            });
        }
    }
}
