
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Contracts.DTOs;
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

        [HttpGet("GetUser'sProfile")]
        public async Task<ActionResult<UserProfileDTO>> GetUser()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            var result = await mediator.Send(new GetUserQuery(userId));

            if (result == null)
                return Ok("There is no information");
            return Ok(result);
        }
    }
}
