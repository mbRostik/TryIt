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
    public class CommentReactionEntityConfiguration : IEntityTypeConfiguration<CommentReaction>
    {
        public void Configure(EntityTypeBuilder<CommentReaction> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
              .IsRequired()
              .ValueGeneratedOnAdd()
              .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasOne(x => x.User)
                .WithMany(x => x.CommentReactions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Comment)
                .WithMany(x => x.CommentReactions)
                .HasForeignKey(x => x.CommentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
