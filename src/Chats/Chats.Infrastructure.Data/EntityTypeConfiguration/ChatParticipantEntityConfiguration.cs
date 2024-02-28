using Chats.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Infrastructure.Data.EntityTypeConfiguration
{
    public class ChatParticipantEntityConfiguration : IEntityTypeConfiguration<ChatParticipant>
    {
        public void Configure(EntityTypeBuilder<ChatParticipant> builder)
        {
            builder.HasKey(cp => new { cp.UserId, cp.ChatId });

            builder.HasOne(cp => cp.User)
               .WithMany(u => u.ChatParticipants)
               .HasForeignKey(cp => cp.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.Chat)
                   .WithMany(c => c.ChatParticipants)
                   .HasForeignKey(cp => cp.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
