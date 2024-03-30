using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Application.Contracts.DTOs;
using Posts.Application.UseCases.Commands;
using Posts.Application.UseCases.Queries;
using Posts.Domain.Entities;
using System.Security.Claims;

namespace Posts.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly Serilog.ILogger logger;

        public PostController(IMediator mediator, Serilog.ILogger logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpGet("GetUserPosts")]
        public async Task<ActionResult<List<GiveProfilePostsDTO>>> GetUserPosts()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await mediator.Send(new GetUserPostsQuery(userId));

            return Ok(result);
        }

        [HttpPost("CreatePost")]
        public async Task<ActionResult> CreatePost([FromBody] CreatePostDTO model)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            model.UserId = userId;

            var result = await mediator.Send(new CreatePostCommand(model));
            if (result)
            {
                return Ok();
            }
            return BadRequest("There was a problem while creating the post");
        }

        [HttpPost("GetsmbPosts")]
        public async Task<ActionResult<List<GiveProfilePostsDTO>>> GetsmbPosts([FromBody] GetSmbPosts model)
        {
            var result = await mediator.Send(new GetsmbPostsQuery(model.ProfileId));
            if (result.Any())
            {
                return Ok(result);
            }
            return BadRequest("There was a problem while creating the post");
        }

    }
}
