using Microsoft.AspNetCore.Identity;

namespace AaronUrkinCodeExample.DataAccessLayer.Security.Entities
{
    /// <summary>
    /// Represents application user entity extending <see cref="IdentityUser"/>
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Country { get; set; }
    }
}
