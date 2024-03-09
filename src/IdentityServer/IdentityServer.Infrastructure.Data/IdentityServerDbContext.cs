using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace IdentityServer.Infrastructure.Data
{
    public class IdentityServerDbContext : IdentityDbContext, IDataProtectionKeyContext
    {
        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options) : base(options)
        {

        }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}