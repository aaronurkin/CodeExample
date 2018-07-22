using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Security;
using AaronUrkinCodeExample.BusinessLogicLayer.Infrastructure;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Security
{
    public interface ISignInManager
    {
        Task<ServiceResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure = false);

        Task<ServiceResult> PasswordSignInAsync(ApplicationUserDto user, string password, bool isPersistent, bool lockoutOnFailure = false);

        Task SignOutAsync();

        bool IsSignedIn(ClaimsPrincipal principal);
    }
}
