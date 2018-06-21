using AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Security;
using AaronUrkinCodeExample.BusinessLogicLayer.Infrastructure;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Security
{
    public interface IUserManager
    {
        Task<ServiceResult> CreateAsync(ApplicationUserDto userDto);

        Task<ServiceResult> CreateAsync(ApplicationUserDto userDto, string password);

        Task<ServiceResult> ConfirmEmailAsync(string userId, string code);

        Task<ServiceResult> GeneratePasswordResetTokenAsync(string email);

        Task<ServiceResult> ResetPasswordAsync(string email, string code, string password);

        string GetUserName(ClaimsPrincipal principal);

        string GetUserFirstName(ClaimsPrincipal principal);

        string GetUserDisplayName(ClaimsPrincipal principal);

        Task<ApplicationUserDto> GetUserAsync(ClaimsPrincipal principal);
    }
}