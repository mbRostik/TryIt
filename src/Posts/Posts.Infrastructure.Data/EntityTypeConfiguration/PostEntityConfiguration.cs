using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Posts.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Posts.Infrastructure.Data.EntityTypeConfiguration
{
    public class PostEntityConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
              .IsRequired()
              .ValueGeneratedOnAdd()
              .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasOne(x=>x.User)
                .WithMany(x=>x.Posts)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Ban)
                .WithOne(x => x.Post)
                .HasForeignKey<Ban>(b => b.BannedPostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x=>x.Files)
                .WithOne(x=>x.Post)
                .HasForeignKey(x=>x.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x=>x.Comments)
                .WithOne(x=>x.Post)
                .HasForeignKey(x=>x.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x=>x.PostReactions)
                .WithOne(x=>x.Post)
                .HasForeignKey(x=>x.PostId)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
