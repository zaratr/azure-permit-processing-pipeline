using Microsoft.EntityFrameworkCore;
using Permit.Api.Models;

namespace Permit.Api.Data
{
    public class PermitDbContext : DbContext
    {
        public PermitDbContext(DbContextOptions<PermitDbContext> options) : base(options)
        {
        }

        public DbSet<PermitRequestMessage> PermitRequests => Set<PermitRequestMessage>();
    }
}
