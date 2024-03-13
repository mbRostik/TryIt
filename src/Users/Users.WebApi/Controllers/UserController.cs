using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;

namespace Users.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpGet("GetUserById")]
        //Temporary
        public async Task<ActionResult<User>> GetUser()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            return Ok();
        }
    }
}
