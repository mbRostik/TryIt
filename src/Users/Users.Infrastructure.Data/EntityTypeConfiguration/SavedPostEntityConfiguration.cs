using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Infrastructure.Data.EntityTypeConfiguration
{
    public class SavedPostEntityConfiguration : IEntityTypeConfiguration<SavedPost>
    {
        public void Configure(EntityTypeBuilder<SavedPost> builder)
        {
            builder.HasKey(mwf => new { mwf.UserId, mwf.PostId });


            builder.HasOne(x => x.User)
                .WithMany(x => x.SavedPosts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Post)
               .WithMany(x => x.SavedPosts)
               .HasForeignKey(x => x.PostId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
