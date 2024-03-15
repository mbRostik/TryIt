using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Users.Domain.Enums;

namespace Users.Infrastructure.Data.EntityTypeConfiguration
{
    public class SexEntityConfiguration : IEntityTypeConfiguration<Sex>
    {
        public void Configure(EntityTypeBuilder<Sex> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasMany(x=>x.Users)
                .WithOne(x=>x.Sex)
                .HasForeignKey(x=>x.SexId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasData(
               new Sex { Id = 1, SexType = SexType.Man },
               new Sex { Id = 2, SexType = SexType.Woman },
               new Sex { Id = 3, SexType = SexType.UnIdentify }
            );
        }
    }
}
