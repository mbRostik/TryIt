
using Chats.Application.Contracts.DTOs;
using Chats.Application.UseCases.Queries;
using Chats.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chats.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger logger;
        public ChatController(IMediator mediator, Serilog.ILogger logger)
        {
           
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpPost("GetChatMessages")]
        public async Task<ActionResult<List<GiveUserChatMessagesDTO>>> GetChatMessages([FromBody] GetChatMessagesDTO request)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (request.ChatId == null)
            {
                logger.Warning($"GetChatMessages called with null ChatId by user {userId}");

                return Ok(null);
            }
            try
            {
                var result = await mediator.Send(new GetChatMessagesQuery(request.ChatId, userId));
                logger.Information($"GetChatMessages successfully retrieved messages for chat {request.ChatId} and user {userId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error retrieving messages for chat {request.ChatId} and user {userId}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO request)
        {
            if (request.MessageContent == null)
            {
                logger.Warning("SendMessage called with null MessageContent");
                return BadRequest("There is no information to be send");
            }
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            logger.Information($"SendMessage initiated by user {userId}");


            return Ok();
        }

        [HttpPost("GetChatId")]
        public async Task<ActionResult<int>> GetChatId([FromBody] GetChatIdDTO request)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            logger.Information($"GetChatId initiated by user {userId} for profile {request.ProfileId}");

            try
            {
                var result = await mediator.Send(new GetChatIdQuery(userId, request.ProfileId));
                logger.Information($"GetChatId successfully retrieved ChatId {result} for user {userId} and profile {request.ProfileId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error retrieving ChatId for user {userId} and profile {request.ProfileId}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}