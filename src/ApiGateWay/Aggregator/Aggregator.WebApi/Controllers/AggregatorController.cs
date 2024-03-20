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
        private readonly ILogger<AggregatorController> _logger;
        private grpcGetUserForChatService UsergrpcService { get; set; }
        private grpcGetUserChatsService ChatgrpcService { get; set; }
        public AggregatorController(ILogger<AggregatorController> logger, grpcGetUserChatsService ChatgrpcService, grpcGetUserForChatService UsergrpcService)
        {

            _logger = logger;
            this.ChatgrpcService = ChatgrpcService;
            this.UsergrpcService = UsergrpcService;
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
            var result = await ChatgrpcService.GetUserChatsAsync(userId, accessToken);

            if (result.Chats.Count == 1 && result.Chats[0].Chatid == 0)
            {
                return Ok();
            }
            List<string> chatsIds = new List<string>();
            foreach (var item in result.Chats)
            {
                chatsIds.Add(item.ContactId);
            }

            var result2 = await UsergrpcService.GetUserChatsAsync(chatsIds, accessToken);
            Console.WriteLine();
            return Ok();

        }

        //Користувач нажимає або на чат або на Повідомлення і повертається моделька з Chat та користувачом (всі повідомлення та профіль користувача)
    }
}
