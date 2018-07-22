using AaronUrkinCodeExample.DataAccessLayer.Logger.Entities;
using Microsoft.EntityFrameworkCore;

namespace AaronUrkinCodeExample.DataAccessLayer.Logger
{
    public class LoggerDbContext : DbContext
    {
        public const string DefaultSchema = "exmpl_log";
        public const string MigrationTableName = "__MigrationHistory";

        public LoggerDbContext(DbContextOptions<LoggerDbContext> options)
            : base(options)
        {
        }

        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(DefaultSchema);

            builder.Entity<LogEntry>().ToTable("LogEntries");

            builder.Entity<LogEntry>()
                .Property(t => t.Logger)
                .HasMaxLength(128)
                .IsRequired();

            builder.Entity<LogEntry>()
                .Property(t => t.Level)
                .HasMaxLength(10)
                .IsRequired();
        }
    }
}
