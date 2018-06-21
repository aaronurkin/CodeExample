using AaronUrkinCodeExample.DataAccessLayer.Security.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AaronUrkinCodeExample.DataAccessLayer.Security
{
    public class SecurityDbContext : IdentityDbContext<ApplicationUser>
    {
        public const string DefaultSchema = "exmpl_security";
        public const string MigrationTableName = "__MigrationHistory";

        public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(DefaultSchema);

            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");

            builder.Entity<ApplicationUser>()
                .Property(u => u.FirstName)
                .HasMaxLength(35)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .Property(u => u.LastName)
                .HasMaxLength(35)
                .IsRequired(false);

            builder.Entity<ApplicationUser>()
                .Property(u => u.Country)
                .HasMaxLength(50)
                .IsRequired(false);
        }
    }
}
