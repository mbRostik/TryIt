using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Infrastructure.Data.EntityTypeConfiguration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
               .ValueGeneratedNever();
            builder.HasMany(x => x.BannedUsers)
                .WithOne(m => m.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(x => x.Email).IsRequired(false);
            builder.Property(x => x.Phone).IsRequired(false);
            builder.Property(x => x.Bio).IsRequired(false);
            builder.Property(x => x.Photo).IsRequired(false);
            builder.Property(x => x.DateOfBirth)
                    .HasDefaultValue(DateTime.MinValue);
            builder.Property(x => x.SexId)
                    .HasDefaultValue(-1);

            builder.HasMany(x => x.BannedBy)
                .WithOne(m => m.BannedByUser)
                .HasForeignKey(x => x.ModeratorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Followers)
                .WithOne(m => m.Follower)
                .HasForeignKey(x => x.FollowerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Posts)
                .WithOne(m => m.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasMany(x => x.Follows)
                .WithOne(m => m.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.SavedPosts)
                .WithOne(m => m.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x=>x.Sex)
                .WithMany(x=>x.Users)
                .HasForeignKey(x=>x.SexId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
