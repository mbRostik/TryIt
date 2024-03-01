using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Infrastructure.Data.EntityTypeConfiguration
{
    public class BanEntityConfiguration : IEntityTypeConfiguration<Ban>
    {
        public void Configure(EntityTypeBuilder<Ban> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
              .IsRequired()
              .ValueGeneratedOnAdd()
              .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);
        
            
            builder.HasOne(x=>x.User)
                .WithMany(x=>x.Bans)
                .HasForeignKey(x=>x.ModeratorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Post)
                .WithOne(x => x.Ban)
                .HasForeignKey<Ban>(x => x.BannedPostId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}