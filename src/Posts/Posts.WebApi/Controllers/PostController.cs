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
            logger.Information("Starting GetUserPosts for UserId {UserId}", userId);

            try
            {
                var result = await mediator.Send(new GetUserPostsQuery(userId));
                logger.Information("Successfully fetched posts for UserId {UserId}", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error fetching posts for UserId {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("CreatePost")]
        public async Task<ActionResult> CreatePost([FromBody] CreatePostDTO model)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            logger.Information("Starting CreatePost for UserId {UserId}", userId);

            model.UserId = userId;

            try
            {
                var result = await mediator.Send(new CreatePostCommand(model));
                if (result)
                {
                    logger.Information("Post created successfully for UserId {UserId}", userId);
                    return Ok();
                }
                logger.Warning("Failed to create post for UserId {UserId}", userId);
                return BadRequest("There was a problem while creating the post");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error creating post for UserId {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("GetsmbPosts")]
        public async Task<ActionResult<List<GiveProfilePostsDTO>>> GetsmbPosts([FromBody] GetSmbPosts model)
        {
            logger.Information("Starting GetsmbPosts for ProfileId {ProfileId}", model.ProfileId);

            try
            {
                var result = await mediator.Send(new GetsmbPostsQuery(model.ProfileId));
                if (result.Any())
                {
                    logger.Information("Successfully fetched smbPosts for ProfileId {ProfileId}", model.ProfileId);
                    return Ok(result);
                }
                else
                {
                    logger.Warning("No posts found for ProfileId {ProfileId}", model.ProfileId);
                    return NotFound("No posts found");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error fetching smbPosts for ProfileId {ProfileId}", model.ProfileId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
