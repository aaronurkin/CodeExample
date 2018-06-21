using AaronUrkinCodeExample.DataAccessLayer.Extensions;
using AaronUrkinCodeExample.DataAccessLayer.Security;
using AaronUrkinCodeExample.DataAccessLayer.Security.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Security
{
    /// <summary>
    /// Represents non-generic <see cref="UserClaimsPrincipalFactory{TUser, TRole}"/>
    /// </summary>
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            ApplicationUserManager userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        /// <summary>
        /// Extends <see cref="ApplicationUser"/> claims with first name and display name claims
        /// </summary>
        /// <param name="user"><see cref="ApplicationUser"/></param>
        /// <returns><see cref="ClaimsIdentity"/> instance extended with additional claims</returns>
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            identity.AddClaim(new Claim(ApplicationClaims.UserFirstNameClaimType, user.FirstName ?? user.Email));
            identity.AddClaim(new Claim(ApplicationClaims.UserDisplayNameClaimType, user.GetDisplayName() ?? user.Email));

            return identity;
        }
    }
}
