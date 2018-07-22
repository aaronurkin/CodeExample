using AaronUrkinCodeExample.DataAccessLayer.Localization.Entities;
using Microsoft.EntityFrameworkCore;

namespace AaronUrkinCodeExample.DataAccessLayer.Localization
{
    public class LocalizationDbContext : DbContext
    {
        public const string DefaultSchema = "exmpl_localization";
        public const string MigrationTableName = "__MigrationHistory";

        public LocalizationDbContext(DbContextOptions<LocalizationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Translation> Translations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(DefaultSchema);

            builder.Entity<Translation>().ToTable("Translations");

            builder.Entity<Translation>()
                .Property(t => t.Scope)
                .HasMaxLength(512)
                .IsRequired();

            builder.Entity<Translation>()
                .Property(t => t.CultureCode)
                .HasMaxLength(17)
                .IsRequired();

            builder.Entity<Translation>()
                .HasIndex(t => new { t.CultureCode, t.Scope, t.Key })
                .IsUnique();
        }
    }
}
