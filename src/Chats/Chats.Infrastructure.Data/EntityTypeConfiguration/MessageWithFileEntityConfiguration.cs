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
    public class MessageWithFileEntityConfiguration : IEntityTypeConfiguration<MessageWithFile>
    {
        public void Configure(EntityTypeBuilder<MessageWithFile> builder)
        {
            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasKey(mwf => new { mwf.FileId, mwf.MessageId });

            builder.HasOne(mwf => mwf.Message)
                   .WithMany(m => m.MessageWithFiles)
                   .HasForeignKey(mwf => mwf.MessageId);

            builder.HasOne(mwf => mwf.File)
                   .WithMany(f => f.MessageWithFiles)
                   .HasForeignKey(mwf => mwf.FileId);
        }
    }
}