using Aggregator.Infrastructure.Policies;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Userforpost;
using Userposts;

namespace Aggregator.Infrastructure.Services.ProtoServices
{
    public class GrpcGetUserForPostsService
    {

        private readonly IConfiguration _configuration;
        private readonly GrpcPolly _grpcPolly;

        public GrpcGetUserForPostsService(IConfiguration configuration, GrpcPolly grpcPolly)
        {
            _configuration = configuration;
            _grpcPolly = grpcPolly;
        }

        public async Task<GetUserForPostResponse> GetUserForPostAsync(string token, string id)
        {
            var headers = new Metadata
            {
                { "Authorization", $"Bearer {token}" }
            };
            var serviceAddress = _configuration["GrpcServices:UserService:Address"];

            return await _grpcPolly.ExponentialGrpcRetry.ExecuteAsync(async () =>
            {
                using var channel = GrpcChannel.ForAddress(serviceAddress);
                var client = new UserForPostService.UserForPostServiceClient(channel);

                var request = new GetUserForPostRequest();
                request.UserId = id;

                return await client.GetUserForPostAsync(request, headers);
            });
        }

    }
}
