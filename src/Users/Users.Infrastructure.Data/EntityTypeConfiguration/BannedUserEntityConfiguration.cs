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
    public class BannedUserEntityConfiguration : IEntityTypeConfiguration<BannedUser>
    {
        public void Configure(EntityTypeBuilder<BannedUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasOne(x => x.User)
                .WithMany(x => x.BannedUsers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.BannedByUser)
                .WithMany(x => x.BannedBy)
                .HasForeignKey(x => x.ModeratorId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
