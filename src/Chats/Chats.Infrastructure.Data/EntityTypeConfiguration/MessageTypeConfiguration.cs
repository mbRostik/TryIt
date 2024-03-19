using Chats.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Infrastructure.Data.EntityTypeConfiguration
{
    public class MessageTypeConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);


            builder.Property(x => x.Content)
                .HasMaxLength(5000);

            builder.Property(x => x.IsRead)
                .IsRequired();

            builder.Property(x=>x.Date)
                .IsRequired();


            builder.HasOne(m => m.User) 
              .WithMany(u => u.Messages) 
              .HasForeignKey(m => m.SenderId) 
              .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(m => m.Chat)
              .WithMany(u => u.Messages)
              .HasForeignKey(m => m.ChatId)
              .OnDelete(DeleteBehavior.NoAction);

            
            builder.HasMany(f => f.MessageWithFiles)
              .WithOne(mf => mf.Message)
              .HasForeignKey(mf => mf.MessageId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}