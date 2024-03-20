using Aggregator.Application.Contracts.DTOs;
using Aggregator.Application.Contracts.Interfaces;
using Aggregator.WebApi.Services.ProtoServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Aggregator.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AggregatorController : ControllerBase
    {
        /////////////////////////////////////////////////
        //
        //
        // РОБИЛОСЬ ПІЗНОЇ НОЧІ. ПОТІМ КОД ПОЗАМІТАЮ XD
        //
        /////////////////////////////////////////////////
        private IChatService ChatService { get; set; }
        public AggregatorController(IChatService ChatService)
        {
            this.ChatService = ChatService;
        }

        [HttpGet("GetUserChats")]
        public async Task<ActionResult<List<GiveUserChatsDTO>>> GetUserChats()
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
            if(userId==null || accessToken == null)
            {
                return null;
            }
            var result = await ChatService.GetUserChats(userId, accessToken);
            return result;
        }
    }
}
