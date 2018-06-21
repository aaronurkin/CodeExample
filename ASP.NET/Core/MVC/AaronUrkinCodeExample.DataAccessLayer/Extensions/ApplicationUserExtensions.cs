using AaronUrkinCodeExample.DataAccessLayer.Security.Entities;
using System.Linq;

namespace AaronUrkinCodeExample.DataAccessLayer.Extensions
{
    /// <summary>
    /// Extends <see cref="ApplicationUser"/> entity
    /// </summary>
    public static class ApplicationUserExtensions
    {
        /// <summary>
        /// Builds display name concatenating first and last name excluding empty entries
        /// </summary>
        /// <param name="user"><see cref="ApplicationUser"/> instance to get display name</param>
        /// <returns>User display name</returns>
        public static string GetDisplayName(this ApplicationUser user)
        {
            if (user != null)
            {
                return string.Join(' ', new[] { user.FirstName, user.LastName }.Where(s => s != null));
            }

            return null;
        }
    }
}
