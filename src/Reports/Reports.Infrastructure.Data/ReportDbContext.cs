using Microsoft.EntityFrameworkCore;
using Reports.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Infrastructure.Data
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Post> Posts { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ReportedPost> ReportedPosts { get; set; }
        public DbSet<ReportedUser> ReportedUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
