using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Subscriptions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriptions.Infrastructure.Data.EntityTypeConfiguration
{
    public class SubscriptionEntityConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.HasOne(x => x.User)
                .WithMany(v => v.Subscriptions)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
