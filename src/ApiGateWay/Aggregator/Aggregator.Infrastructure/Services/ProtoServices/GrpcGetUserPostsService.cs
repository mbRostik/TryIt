using Aggregator.Infrastructure.Policies;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Userposts;

namespace Aggregator.Infrastructure.Services.ProtoServices
{
    public class GrpcGetUserPostsService
    {
        private readonly IConfiguration _configuration;
        private readonly GrpcPolly _grpcPolly;

        public GrpcGetUserPostsService(IConfiguration configuration, GrpcPolly grpcPolly)
        {
            _configuration = configuration;
            _grpcPolly = grpcPolly;
        }

        public async Task<GetUserPostsResponse> GetUserForPostAsync(string UserId, string token)
        {
            var headers = new Metadata
            {
                { "Authorization", $"Bearer {token}" }
            };
            var serviceAddress = _configuration["GrpcServices:PostService:Address"];

            return await _grpcPolly.ExponentialGrpcRetry.ExecuteAsync(async () =>
            {
                using var channel = GrpcChannel.ForAddress(serviceAddress);
                var client = new UserPostsService.UserPostsServiceClient(channel);

                var request = new GetUserPostsRequest();
                request.UserId = UserId;

                return await client.GetUserPostsAsync(request, headers);
            });
        }
    }
}
