using Aggregator.WebApi.Services.ProtoServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aggregator.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AggregatorController : ControllerBase
    {

        private readonly ILogger<AggregatorController> _logger;

        private grpcGetUserChatsService grpcService { get; set; }
        public AggregatorController(ILogger<AggregatorController> logger, grpcGetUserChatsService grpcService)
        {

            _logger = logger;
            this.grpcService = grpcService;
        }

        [HttpGet("Temp")]
        public async Task<IActionResult> Get()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            string accessToken = null;
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                var headerValue = authorizationHeader.FirstOrDefault();
                if (headerValue?.StartsWith("Bearer ") == true)
                {
                    accessToken = headerValue.Substring("Bearer ".Length).Trim();
                }
            }
            Console.WriteLine(accessToken);
            await grpcService.GetUserChatsAsync("7fc89f0e-c0cc-4b5b-aa3c-5c9d6becec57", accessToken);
            return Ok();
        }
    }
}
