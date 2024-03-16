// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityServerHost.Pages.Login;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using static System.Net.WebRequestMethods;

namespace IdentityServerHost.Pages.Register;

[SecurityHeaders]
[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public RegisterModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; } = "https://localhost:7174/Account/Login";
    public async Task<IActionResult> OnGet(string returnUrl = null)
    {
        if (User.Identity.IsAuthenticated)
        {
            return LocalRedirect("~/");
        }
        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        if (User.Identity.IsAuthenticated)
        {
            return LocalRedirect("~/");
        }

        if (ModelState.IsValid)
        {
            var chechkEmail = await _userManager.FindByEmailAsync(Input.Email);
            if (chechkEmail != null)
            {
                ModelState.AddModelError(string.Empty, "This email is already taken");
                return Page();
            }
            else
            {
                var user = new IdentityUser { UserName = Input.Username, Email = Input.Email, EmailConfirmed = false, TwoFactorEnabled = false };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    //Створюємо Юзера та токен підтвердження
                    var CreatedUser = await _userManager.FindByEmailAsync(user.Email);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //силка для підтверження де міститься айді користувача, токен та returnUrl яку ми достали з LoginPage, коли натиснули Зареєструватись

                    var returnUrlQuery = !string.IsNullOrEmpty(ReturnUrl) ? $"&returnUrl={Uri.EscapeDataString(ReturnUrl)}" : string.Empty;
                    var callbackUrl = $"https://localhost:7174/ConfirmEmail?id={CreatedUser.Id}&token={Uri.EscapeDataString(code)}{returnUrlQuery}";

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("rost.daskalyuk@gmail.com");
                    message.Subject = "TryIt verification";
                    message.To.Add(new MailAddress(user.Email));
                    message.Body = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.";
                    message.IsBodyHtml = true;

                    var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("rost.daskalyuk@gmail.com", "ueexgknctftcjnpo"),
                        EnableSsl = true

                    };

                    smtpClient.Send(message);
                    if (ReturnUrl != null && ReturnUrl != "")
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    return Redirect("~/");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
           
        }

        return Page();
    }

}



