using Microsoft.EntityFrameworkCore;
using Posts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Infrastructure.Data
{
    public class PostDbContext : DbContext
    {
        public PostDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Ban> Bans { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReaction> CommentReactions { get; set; }
        public DbSet<PFile> PFiles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostReaction> PostReactionts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
