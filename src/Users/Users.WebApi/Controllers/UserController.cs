using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;

namespace Users.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetAllPosts")]
        public async Task<ActionResult<List<Post>>> GetItems()
        {

            var result = await mediator.Send(new GetAllPostsQuery());

            return Ok(result);

        }
    }
}
