using IdentityServerHost.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;

namespace IdentityServer.WebApi.Pages.ConfirmEmail
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string token)
        {
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

            var result = await _userManager.ConfirmEmailAsync(user, token);
            StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            return Page();
        }
    }
}