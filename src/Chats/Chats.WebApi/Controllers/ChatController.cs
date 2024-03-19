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

        public ChatController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetUser'sChats")]
        public async Task<ActionResult<List<Chat>>> GetChats()
        {
            return Ok();
        }

        [HttpPost("GetChatMessages")]
        public async Task<ActionResult<List<Message>>> GetChatMessages([FromBody] GetChatMessagesDTO request)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (request.ChatId == null)
            {
                return Ok(null);
            }
            var result = await mediator.Send(new GetChatMessagesQuery(request.ChatId, userId));
            return Ok(result);
        }


        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO request)
        {
            if (request.Data == null && request.MessageContent == null)
            {
                return BadRequest("There is no information to be send");
            }
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return Ok();
        }

        [HttpPost("GetChatId")]
        public async Task<ActionResult<int>> GetChatId([FromBody] GetChatIdDTO request)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result = await mediator.Send(new GetChatIdQuery(userId, request.ProfileId));

            Console.WriteLine(result + "  ID WHICH WOULD BE RETUNED");
            return Ok(result);
        }

        //[HttpGet("GetAllUserChats")]
        //public async Task<ActionResult<List<>> GetAllUserChats(){
        //    return null;
        //}
}