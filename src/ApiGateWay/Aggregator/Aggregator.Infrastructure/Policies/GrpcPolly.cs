using Polly.Retry;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;

namespace Aggregator.Infrastructure.Policies
{
    public class GrpcPolly
    {
        public AsyncRetryPolicy ExponentialGrpcRetry { get; }

        public AsyncRetryPolicy ImmediateGrpcRetry { get; }
        public AsyncRetryPolicy LinearGrpcRetry { get; }

        private readonly Serilog.ILogger _logger;

        public GrpcPolly(Serilog.ILogger logger)
        {
            _logger = logger;
            var gRpcErrors = new StatusCode[] {
            StatusCode.DeadlineExceeded,
            StatusCode.Internal,
            StatusCode.NotFound,
            StatusCode.ResourceExhausted,
            StatusCode.Unavailable,
            StatusCode.Unknown
        };

            ImmediateGrpcRetry = Policy
                .Handle<RpcException>(ex => gRpcErrors.Contains(ex.StatusCode))
                .RetryAsync(3, onRetry: (exception, retryCount, context) =>
                {
                    logger.Warning($"Immediate retry {retryCount} for {exception}");
                });

            LinearGrpcRetry = Policy
                .Handle<RpcException>(ex => gRpcErrors.Contains(ex.StatusCode))
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        logger.Warning($"Request failed with {exception}. Waiting {timespan} before next retry. Retry attempt {retryCount}");
                    });

            ExponentialGrpcRetry = Policy
                .Handle<RpcException>(ex => gRpcErrors.Contains(ex.StatusCode))
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        logger.Warning($"Request failed with {exception}. Waiting {timespan} before next retry. Retry attempt {retryCount}");
                    });
        }
    }
}