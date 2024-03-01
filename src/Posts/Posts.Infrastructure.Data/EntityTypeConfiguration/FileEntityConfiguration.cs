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
    public class FileEntityConfiguration : IEntityTypeConfiguration<PFile>
    {
        public void Configure(EntityTypeBuilder<PFile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
              .IsRequired()
              .ValueGeneratedOnAdd()
              .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasOne(x => x.Post)
                .WithMany(x => x.Files)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}