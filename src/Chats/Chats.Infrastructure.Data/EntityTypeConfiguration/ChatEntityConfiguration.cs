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
    public class ChatEntityConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasMany(x => x.ChatParticipants)
                .WithOne(c => c.Chat)
                .HasForeignKey(v => v.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x=>x.LastMessage)
                .WithMany(c=>c.Chats)
                .HasForeignKey(c=>c.LastMessageId) 
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Messages)
                .WithOne(v => v.Chat)
                .HasForeignKey(b => b.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}