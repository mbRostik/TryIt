using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chats.Domain.Entities;

namespace Chats.Infrastructure.Data.EntityTypeConfiguration
{
    public class FileEntityConfiguration : IEntityTypeConfiguration<CFile>
    {
        public void Configure(EntityTypeBuilder<CFile> builder)
        {

            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasMany(f => f.MessageWithFiles)
              .WithOne(mf => mf.File)
              .HasForeignKey(mf => mf.FileId);

            builder.Property(x => x.Data)
                .IsRequired()
                 .HasMaxLength(10800333);

            builder.Property(x=>x.Name)
                .IsRequired()
                .HasMaxLength (70);

            builder.Property(x => x.Data)
                .IsRequired();

        }
    }
}
