using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Security;
using AaronUrkinCodeExample.BusinessLogicLayer.Infrastructure;
using AaronUrkinCodeExample.DataAccessLayer.Security;
using AaronUrkinCodeExample.DataAccessLayer.Security.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Security
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IMapper mapper;
        private readonly ApplicationUserManager userManager;
        private readonly ApplicationSignInManager signInManager;
        private readonly ILogger<ApplicationUserService> logger;

        public ApplicationUserService(
            IMapper mapper,
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            ILogger<ApplicationUserService> logger)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        /// <summary>
        /// Creates the specified user in the backing store with no password
        /// </summary>
        /// <param name="userDto">The data transfer object containing user data to create</param>
        /// <returns>The <see cref="ServiceResult"/> result.</returns>
        public virtual async Task<ServiceResult> CreateAsync(ApplicationUserDto userDto)
        {
            var user = this.mapper.Map<ApplicationUser>(userDto);
            var result = await this.userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                this.mapper.Map(user, userDto, opts => opts.AfterMap(async (u, dto) => { dto.EmailConfirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(u); }));
                return new ServiceResult(result);
            }

            return new ServiceResult(result);
        }

        /// <summary>
        /// Creates the specified user in the backing store with given password
        /// </summary>
        /// <param name="userDto">The data transfer object containing user data to create</param>
        /// <param name="password">The password for the user to hash and store.</param>
        /// <returns>The <see cref="ServiceResult"/> result.</returns>
        public virtual async Task<ServiceResult> CreateAsync(ApplicationUserDto userDto, string password)
        {
            var user = this.mapper.Map<ApplicationUser>(userDto);
            var result = await this.userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                this.mapper.Map(user, userDto, opts => opts.AfterMap(async (u, dto) => { dto.EmailConfirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(u); }));
                return new ServiceResult(result);
            }

            return new ServiceResult(result);
        }

        /// <summary>
        /// Validates that an email confirmation token matches the specified user.
        /// </summary>
        /// <param name="userId">The user identifier to find user to validate the token against.</param>
        /// <param name="code">The email confirmation token to validate.</param>
        /// <returns>The <see cref="ServiceResult"/> result.</returns>
        public virtual async Task<ServiceResult> ConfirmEmailAsync(string userId, string code)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }

            var result = await this.userManager.ConfirmEmailAsync(user, code);

            return new ServiceResult(result);
        }

        /// <summary>
        /// Generates a password reset token for the specified user, using the configured password reset token provider.
        /// </summary>
        /// <param name="email">The email to find a user to generate a password reset token for.</param>
        /// <returns>The <see cref="ServiceResult"/> result containing generated token.</returns>
        public virtual async Task<ServiceResult> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);

            if (user == null || !(await this.userManager.IsEmailConfirmedAsync(user)))
            {
                return ServiceResult.Failed();
            }

            var token = await this.userManager.GeneratePasswordResetTokenAsync(user);
            var data = new ForgotPasswordDto
            {
                UserId = user.Id,
                Token = token
            };

            return ServiceResult.Succeed(data);
        }

        /// <summary>
        /// Resets the user's password to the specified newPassword after validating the given password reset token.
        /// </summary>
        /// <param name="email">The user email whose password should be reset.</param>
        /// <param name="code">The password reset token to verify.</param>
        /// <param name="password">The new password to set if reset token verification fails.</param>
        /// <returns>The object containing result for the reset password.</returns>
        public virtual async Task<ServiceResult> ResetPasswordAsync(string email, string code, string password)
        {
            var user = await this.userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return ServiceResult.Failed();
            }

            var result = await this.userManager.ResetPasswordAsync(user, code, password);

            return new ServiceResult(result);
        }

        /// <summary>
        /// Returns the Name claim value if present otherwise returns null.
        /// </summary>
        /// <param name="principal"><see cref="ClaimsPrincipal"/> instance.</param>
        /// <returns>Name claim is identified by <see cref="ClaimsIdentity.DefaultNameClaimType"/>.</returns>
        public virtual string GetUserName(ClaimsPrincipal principal)
        {
            return this.userManager.GetUserName(principal);
        }

        /// <summary>
        /// Returns the First name claim value if present otherwise returns null.
        /// </summary>
        /// <param name="principal"><see cref="ClaimsPrincipal"/> instance.</param>
        /// <returns>First name claim is identified by <see cref="ApplicationClaims.UserFirstNameClaimType"/>.</returns>
        public virtual string GetUserFirstName(ClaimsPrincipal principal)
        {
            var firstName = principal.FindFirst(ApplicationClaims.UserFirstNameClaimType).Value;

            if (string.IsNullOrEmpty(firstName))
            {
                return this.GetUserName(principal);
            }

            return firstName;
        }

        /// <summary>
        /// Returns the Display name claim value if present otherwise returns null.
        /// </summary>
        /// <param name="principal"><see cref="ClaimsPrincipal"/> instance.</param>
        /// <returns>Display name claim is identified by <see cref="ApplicationClaims.UserDisplayNameClaimType"/>.</returns>
        public virtual string GetUserDisplayName(ClaimsPrincipal principal)
        {
            var displayName = principal.FindFirst(ApplicationClaims.UserDisplayNameClaimType).Value;

            if (string.IsNullOrEmpty(displayName))
            {
                return this.GetUserName(principal);
            }

            return displayName;
        }

        /// <summary>
        /// Retrieves user entity and maps it to dto.
        /// </summary>
        /// <param name="principal">User's claims</param>
        /// <returns>Instanse of <see cref="ApplicationUserDto"/></returns>
        public virtual async Task<ApplicationUserDto> GetUserAsync(ClaimsPrincipal principal)
        {
            var user = await this.userManager.GetUserAsync(principal);

            return this.mapper.Map<ApplicationUserDto>(user);
        }

        /// <summary>
        /// Attempts to sign in the specified userName and password combination as an asynchronous operation
        /// </summary>
        /// <param name="email">The user email to sign in.</param>
        /// <param name="password">The password to attempt to sign in with.</param>
        /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed</param>
        /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>The object containing result for the sign-in attempt.</returns>
        public virtual async Task<ServiceResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure = false)
        {
            var result = await this.signInManager.PasswordSignInAsync(email, password, isPersistent, lockoutOnFailure);
            return this.PasswordSignInResult(result, email);
        }

        /// <summary>
        /// Attempts to sign in the specified user and password combination using signInManager
        /// </summary>
        /// <param name="userDto">The data transfer object representing user to sign in.</param>
        /// <param name="password">The password to attempt to sign in with.</param>
        /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
        /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>The object containing result for the sign-in attempt.</returns>
        public virtual async Task<ServiceResult> PasswordSignInAsync(ApplicationUserDto userDto, string password, bool isPersistent, bool lockoutOnFailure = false)
        {
            var user = this.mapper.Map<ApplicationUser>(userDto);
            var result = await this.signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);

            return this.PasswordSignInResult(result, userDto.Email);
        }

        /// <summary>
        /// Signs the current user out of the application.
        /// </summary>
        /// <returns></returns>
        public virtual Task SignOutAsync()
        {
            return this.signInManager.SignOutAsync();
        }

        /// <summary>
        /// Returns true if the principal has an identity with the application cookie identity
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance.</param>
        /// <returns>True if the user is logged in with identity.</returns>
        public virtual bool IsSignedIn(ClaimsPrincipal principal)
        {
            return this.signInManager.IsSignedIn(principal);
        }

        /// <summary>
        /// Resolves the response using specified <see cref="SignInResult"/>
        /// </summary>
        /// <param name="result">Specified <see cref="SignInResult"/></param>
        /// <param name="email">The user email the attempt to sign was for</param>
        /// <returns>The <see cref="ServiceResult"/> for the sign-in attempt.</returns>
        private ServiceResult PasswordSignInResult(SignInResult result, string email)
        {
            if (!result.Succeeded)
            {
                string format = "Invalid {0} login attempt.";

                if (result.IsLockedOut)
                {
                    format = "Account {0} is locked out";
                }
                else if (result.IsNotAllowed)
                {
                    format = "Login for {0} is not allowed";
                }
                else if (result.RequiresTwoFactor)
                {
                    format = "Account {0} requires two factor authentication";
                }

                this.logger.LogWarning(format, email);
            }

            return new ServiceResult(result);
        }
    }
}
