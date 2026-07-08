using Microsoft.EntityFrameworkCore;
using Permit.Api.Models;

namespace Permit.Api.Data
{
    /// <summary>
    /// EF Core context for persisted permit applications.
    ///
    /// Registered in Program.cs with an in-memory provider for local dev/test
    /// (no SQL Server required to run the API). A production deploy would swap
    /// the provider for UseSqlServer/UsePostgres.
    /// </summary>
    public class PermitDbContext : DbContext
    {
        public PermitDbContext(DbContextOptions<PermitDbContext> options) : base(options)
        {
        }

        public DbSet<PermitApplication> PermitApplications => Set<PermitApplication>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed an example row so the dashboard's GET /permits has data to
            // render on first run (the in-memory provider starts empty).
            modelBuilder.Entity<PermitApplication>().HasData(
                new PermitApplication
                {
                    Id = 1,
                    ApplicationId = 1001,
                    ApplicantEmail = "demo@example.com",
                    LicenseType = "General Business",
                    Status = PermitStatus.Submitted,
                    CreatedAt = new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc),
                },
                new PermitApplication
                {
                    Id = 2,
                    ApplicationId = 1002,
                    ApplicantEmail = "contractor@example.com",
                    LicenseType = "Electrical Contractor",
                    Status = PermitStatus.Reviewing,
                    CreatedAt = new DateTime(2026, 7, 2, 9, 30, 0, DateTimeKind.Utc),
                });
        }
    }
}
