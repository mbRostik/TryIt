using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Application.UseCases.Commands;
using Posts.Application.UseCases.Queries;
using Posts.Domain.Entities;

namespace Posts.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IMediator mediator;

        public PostController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetAllPosts")]
        public async Task<ActionResult<List<Post>>> GetItems()
        {

            var result = await mediator.Send(new GetAllPostsQuery());
            return Ok(result);
        }


        [HttpPut("CreateTemporaryPost")]
        public async Task<ActionResult<Post>> PutPost()
        {
            Post temp = new Post
            {
                Title = "Temp",
                Content = "Nema",
                UserId = "1",
                Date = DateTime.Now
            };

            var result = await mediator.Send(new CreatePostCommand(temp));
           
            return Ok(result);

        }

    }
}
