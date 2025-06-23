using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Posts.Domain.Entities;

namespace Posts.Infrastructure.Data.EntityTypeConfiguration
{
    public class PostWithPhotoCategoriesConfiguration : IEntityTypeConfiguration<PostWithPhotoCategories>
    {
        public void Configure(EntityTypeBuilder<PostWithPhotoCategories> builder)
        {
            builder.HasKey(cp => new { cp.PostId, cp.PostPhotoCategoryId });

            builder.HasOne(cp => cp.Post)
               .WithMany(u => u.PostWithPhotoCategories)
               .HasForeignKey(cp => cp.PostId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.PostPhotoCategory)
                   .WithMany(c => c.PostWithPhotoCategories)
                   .HasForeignKey(cp => cp.PostPhotoCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

