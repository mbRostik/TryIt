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
    public class PostWithTextCategoriesConfiguration : IEntityTypeConfiguration<PostWithTextCategories>
    {
        public void Configure(EntityTypeBuilder<PostWithTextCategories> builder)
        {
            builder.HasKey(cp => new { cp.PostId, cp.PostTextCategoryId });

            builder.HasOne(cp => cp.Post)
               .WithMany(u => u.PostWithTextCategories)
               .HasForeignKey(cp => cp.PostId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.PostTextCategory)
                   .WithMany(c => c.PostWithTextCategories)
                   .HasForeignKey(cp => cp.PostTextCategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
