using Aggregator.Infrastructure.Policies;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Userchats;

namespace Aggregator.WebApi.Services.ProtoServices
{
    public class GrpcGetUserChatsService
    {
        private readonly IConfiguration _configuration;
        private readonly GrpcPolly _grpcPolly;

        public GrpcGetUserChatsService(IConfiguration configuration, GrpcPolly grpcPolly)
        {
            _configuration = configuration;
            _grpcPolly = grpcPolly;
        }
        public async Task<GetUserChatsResponse> GetUserChatsAsync(string userId, string token)
        {
            var headers = new Metadata
                {
                    { "Authorization", $"Bearer {token}" }
                };
            var serviceAddress = _configuration["GrpcServices:ChatService:Address"];

            return await _grpcPolly.ExponentialGrpcRetry.ExecuteAsync(async () =>
            {
                using var channel = GrpcChannel.ForAddress(serviceAddress);

                var client = new UserChatsService.UserChatsServiceClient(channel);

                var request = new GetUserChatsRequest { UserId = userId };

                var response = await client.GetUserChatsAsync(request);

                return response;
            });

        }
    }
}
