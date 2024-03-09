using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
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
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEventService _events;
        public TemporaryController(UserManager<IdentityUser> userManager, IPublishEndpoint publisher,
            SignInManager<IdentityUser> signInManager,
            IEventService events)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _events = events;
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

