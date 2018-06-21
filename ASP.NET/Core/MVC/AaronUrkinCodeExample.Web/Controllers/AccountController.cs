using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Security;
using AaronUrkinCodeExample.BusinessLogicLayer.Extensions;
using AaronUrkinCodeExample.BusinessLogicLayer.Services;
using AaronUrkinCodeExample.BusinessLogicLayer.Services.Security;
using AaronUrkinCodeExample.Web.Extensions;
using AaronUrkinCodeExample.Web.Models.AccountViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IEmailSender emailSender;
        private readonly IApplicationUserService service;
        private readonly IStringLocalizer<AccountController> translate;

        public AccountController(
            IMapper mapper,
            IEmailSender emailSender,
            IApplicationUserService service,
            ILogger<AccountController> logger,
            IStringLocalizer<AccountController> localizer)
            : base(mapper, logger)
        {
            this.service = service;
            this.translate = localizer;
            this.emailSender = emailSender;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string to = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = to;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string to = null)
        {
            ViewData["ReturnUrl"] = to;

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await this.service.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                var message = result.Data?.ToString();

                if (result.Succeeded)
                {
                    this.Logger.LogInformation("{0} logged in", model.Email);
                    return RedirectToLocal(to);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, message ?? this.translate["Invalid login attempt."]);
                    return View(model);
                }
            }

            this.Logger.LogError("{0} login failed", model.Email);
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string to = null)
        {
            ViewData["ReturnUrl"] = to;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string to = null)
        {
            if (ModelState.IsValid)
            {
                var user = this.AutoMapper.Map<ApplicationUserDto>(model);
                var result = await this.service.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    this.Logger.LogInformation("User created a new account ({0}) with password.", user.Email);

                    var callbackUrl = Url.EmailConfirmationLink(user.Id, user.EmailConfirmationToken, to, Request.Scheme);

                    //// TODO: implement email sending. Store settings within appsettings.json
                    await this.emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    //// TODO: Remove this after emailSender implementation
                    this.Logger.LogWarning("Confirmation link was not sent to {0}, because email sender is not implemented yet.", model.Email);

                    //// TODO: Remove this. Demonstration purposes ONLY!
                    ViewData["ConfirmationLink"] = callbackUrl;

                    return View("ConfirmWithoutEmail");
                }

                this.Logger.LogError("User {0} was not created", model.Email);
                this.AddErrors(result.Errors);
            }

            ViewData["ReturnUrl"] = to;

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = this.service.GetUserName(this.User);

            await this.service.SignOutAsync();
            this.Logger.LogInformation("{0} logged out.", user);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string to)
        {
            ViewData["ReturnUrl"] = to;

            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var result = await this.service.ConfirmEmailAsync(userId, code);

            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await this.service.GeneratePasswordResetTokenAsync(model.Email);

                if (!(result.Succeeded && result.Data is ForgotPasswordDto dto))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var callbackUrl = Url.ResetPasswordCallbackLink(dto.UserId, dto.Token, Request.Scheme);

                // TODO: implement email sending. Store settings within appsettings.json
                await this.emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                TempData["ConfirmationLink"] = callbackUrl;

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await this.service.ResetPasswordAsync(model.Email, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            this.Logger.LogWarning("Password was not reset for user {0}.", model.Email);
            this.AddErrors(result.Errors);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                this.Logger.LogError("Error: {0}; Description: {1}", error.Code, error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}