using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reports.Domain;

namespace Reports.Infrastructure.Data.EntityTypeConfiguration
{
    public class PostEntityConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.HasMany(x => x.ReportedPosts)
                .WithOne(x => x.Post)
                .HasForeignKey(x => x.ReportedPostId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}