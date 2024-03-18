using IdentityServerHost.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using MassTransit;
using Duende.IdentityServer.Test;
using MessageBus.Messages.IdentityServerService;
using IdentityServerHost.Pages.Login;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Duende.IdentityServer.Events;
using Duende.IdentityServer;

namespace IdentityServer.WebApi.Pages.ConfirmEmail
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPublishEndpoint _publisher;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IIdentityProviderStore _identityProviderStore;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public ViewModel View { get; set; } = default!;
        public IndexModel(UserManager<IdentityUser> userManager, IPublishEndpoint publisher, IIdentityServerInteractionService interaction,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IEventService events, SignInManager<IdentityUser> signInManager,
        IConfiguration configuration)
        {
            _userManager = userManager;
            _publisher = publisher;
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _identityProviderStore = identityProviderStore;
            _events = events;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string token, string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return LocalRedirect("~/");
            }
            if (id == null || token == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                StatusMessage = "Error: Unable to load user.";
                return Page();
            }

            //Тут робимо всі дії що при Login, після чого нас перекине на Consent ну і далі на реакт веб-додаток
            await BuildModelAsync(returnUrl);

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var resultConfirmation = await _userManager.ConfirmEmailAsync(user, token);
            if (!resultConfirmation.Succeeded)
            {
                StatusMessage = "Error confirming your email.";
                return Page();
            }
            IdentityUserCreatedEvent creationEvent = new IdentityUserCreatedEvent
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserName = user.UserName
            };

            await _publisher.Publish(creationEvent);


            await _signInManager.SignInAsync(user, true);

            await _events.RaiseAsync(new UserLoginSuccessEvent(user!.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));
            IdentityServerHost.Pages.Telemetry.Metrics.UserLogin(context?.Client.ClientId, IdentityServerConstants.LocalIdentityProvider);

            if (context != null)
            {
                // This "can't happen", because if the ReturnUrl was null, then the context would be null
                ArgumentNullException.ThrowIfNull(returnUrl, nameof(returnUrl));

                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(returnUrl);
                }

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(returnUrl ?? "~/");
            }

            // request for a local page
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else if (string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(_configuration.GetValue<string>("Links:ReactLink"));
            }
            else
            {
                // user might have clicked on a malicious link - should be logged
                throw new ArgumentException("invalid return URL");
            }
        }

        private async Task BuildModelAsync(string? returnUrl)
        {

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                View = new ViewModel
                {
                    EnableLocalLogin = local,
                };

                if (!local)
                {
                    View.ExternalProviders = new[] { new ViewModel.ExternalProvider(authenticationScheme: context.IdP) };
                }

                return;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ViewModel.ExternalProvider
                (
                    authenticationScheme: x.Name,
                    displayName: x.DisplayName ?? x.Name
                )).ToList();

            var dynamicSchemes = (await _identityProviderStore.GetAllSchemeNamesAsync())
                .Where(x => x.Enabled)
                .Select(x => new ViewModel.ExternalProvider
                (
                    authenticationScheme: x.Scheme,
                    displayName: x.DisplayName ?? x.Scheme
                ));
            providers.AddRange(dynamicSchemes);


            var allowLocal = true;
            var client = context?.Client;
            if (client != null)
            {
                allowLocal = client.EnableLocalLogin;
                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Count != 0)
                {
                    providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                }
            }

            View = new ViewModel
            {
                AllowRememberLogin = LoginOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
                ExternalProviders = providers.ToArray()
            };
        }
    }
}