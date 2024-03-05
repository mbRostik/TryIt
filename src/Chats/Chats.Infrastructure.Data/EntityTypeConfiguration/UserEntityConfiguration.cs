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
    public class UserEntityConfiguration:IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
              .ValueGeneratedNever();
            builder.HasMany(x=>x.Messages)
                .WithOne(m=>m.User)
                .HasForeignKey(x=>x.SenderId)
                 .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.ChatParticipants)
               .WithOne(cp => cp.User)
               .HasForeignKey(cp => cp.UserId)
               .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
