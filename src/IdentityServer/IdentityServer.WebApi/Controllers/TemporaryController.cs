using MassTransit;
using MessageBus.Messages.IdentityServerService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.WebApi.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class TemporaryController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPublishEndpoint _publisher;

        public TemporaryController(UserManager<IdentityUser> userManager, IPublishEndpoint publisher)
        {
            _userManager = userManager;
            _publisher = publisher;
        }

        [HttpPost("PostSmth")]
        public async Task<IActionResult> AddUser(string UserName)
        {
            IdentityUser user = new IdentityUser { UserName = UserName, Email = UserName };
            var result = await _userManager.CreateAsync(user, "TempPassword1!");
            if (result.Succeeded)
            {
                var tempUser = await _userManager.FindByEmailAsync(UserName);


                Console.WriteLine("Publishing User");
                IdentityUserCreatedEvent creationEvent = new IdentityUserCreatedEvent
                {
                    UserId = tempUser.Id,
                    UserEmail = tempUser.Email,
                    UserName = tempUser.UserName
                };

                await _publisher.Publish(creationEvent);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
