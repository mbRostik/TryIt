using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Reports.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Infrastructure.Data.EntityTypeConfiguration
{
    public class ReportedUserEntityConfiguration : IEntityTypeConfiguration<ReportedUser>
    {
        public void Configure(EntityTypeBuilder<ReportedUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasOne(x=>x.User)
                .WithMany(x=>x.Users)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x=>x.RepUser)
                .WithMany(x=>x.RepUsers)
                .HasForeignKey(x=>x.ReportedUserId)
                .OnDelete(DeleteBehavior.NoAction);



        }
    }
}