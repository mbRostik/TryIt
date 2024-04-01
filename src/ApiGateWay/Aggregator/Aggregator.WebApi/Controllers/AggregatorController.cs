using Aggregator.Application.Contracts.DTOs;
using Aggregator.Application.Contracts.Interfaces;
using Aggregator.WebApi.Services.ProtoServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
namespace Aggregator.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AggregatorController : ControllerBase
    {
        private IChatService ChatService { get; set; }
        private IPostService PostService { get; set; }


        private readonly Serilog.ILogger logger;
        public AggregatorController(IChatService ChatService, Serilog.ILogger logger, IPostService PostService)
        {
            this.ChatService = ChatService;
            this.logger = logger;
            this.PostService = PostService;
        }

        [HttpGet("GetUserChats")]
        public async Task<ActionResult<List<GiveUserChatsDTO>>> GetUserChats()
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                logger.Information("Attempting to get user chats for UserId: {UserId}", userId);

                string accessToken = null;
                if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    var headerValue = authorizationHeader.FirstOrDefault();
                    if (headerValue?.StartsWith("Bearer ") == true)
                    {
                        accessToken = headerValue.Substring("Bearer ".Length).Trim();
                        logger.Information("Extracted access token for UserId: {UserId}", userId);
                    }
                }

                if (userId == null || accessToken == null)
                {
                    logger.Warning("Failed to retrieve user chats for UserId: {UserId} due to missing userId or accessToken", userId);
                    return BadRequest("Missing user identity or access token");
                }

                var result = await ChatService.GetUserChats(userId, accessToken);
                logger.Information("Successfully retrieved user chats for UserId: {UserId}", userId);

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error retrieving user chats for UserId: {UserId}", HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetFollowedPosts")]
        public async Task<ActionResult<List<GiveFollowedPostsDTO>>> GetFollowedPosts()
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                logger.Information("Attempting to GetFollowedPosts for UserId: {UserId}", userId);

                string accessToken = null;
                if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    var headerValue = authorizationHeader.FirstOrDefault();
                    if (headerValue?.StartsWith("Bearer ") == true)
                    {
                        accessToken = headerValue.Substring("Bearer ".Length).Trim();
                        logger.Information("Extracted access token for UserId: {UserId}", userId);
                    }
                }

                if (userId == null || accessToken == null)
                {
                    logger.Warning("Failed to retrieve GetFollowedPosts for UserId: {UserId} due to missing userId or accessToken", userId);
                    return BadRequest("Missing user identity or access token");
                }

                var result = await PostService.GetFollowedPosts(userId, accessToken);
                logger.Information("Successfully retrieved GetFollowedPosts for UserId: {UserId}", userId);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                logger.Error(ex, "Error retrieving GetFollowedPosts for UserId: {UserId}", HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
