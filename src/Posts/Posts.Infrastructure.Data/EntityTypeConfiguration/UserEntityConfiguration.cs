using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Posts.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Posts.Infrastructure.Data.EntityTypeConfiguration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
              .ValueGeneratedNever();


            builder.HasMany(x => x.Comments)
                .WithOne(m => m.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.PostReactions)
                .WithOne(m => m.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Bans)
                .WithOne(m => m.User)
                .HasForeignKey(x => x.ModeratorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.CommentReactions)
               .WithOne(m => m.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
