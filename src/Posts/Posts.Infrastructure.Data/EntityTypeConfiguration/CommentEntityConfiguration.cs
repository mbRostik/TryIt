using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Posts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Infrastructure.Data.EntityTypeConfiguration
{
    public class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
              .IsRequired()
              .ValueGeneratedOnAdd()
              .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasOne(x=>x.Post)
                .WithMany(x=>x.Comments)
                .HasForeignKey(x=>x.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Replies)
                .WithOne(x => x.ParentComment)
                .HasForeignKey(x => x.ParentCommentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x=>x.ParentComment)
                .WithMany(x=>x.Replies)
                .HasForeignKey(x=>x.ParentCommentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}