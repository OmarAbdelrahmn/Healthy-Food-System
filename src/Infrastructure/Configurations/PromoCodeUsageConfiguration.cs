using Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations;
public class PromoCodeUsageConfiguration : IEntityTypeConfiguration<PromoCodeUsage>
{
    public void Configure(EntityTypeBuilder<PromoCodeUsage> builder)
    {
        builder.ToTable("PromoCodeUsages");

        builder.HasKey(pcu => pcu.Id);

        builder.Property(pcu => pcu.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(pcu => pcu.UsedAt)
            .IsRequired();
        builder.HasOne(pcu => pcu.PromoCode)
               .WithMany(pc => pc.Usages)
               .HasForeignKey(pcu => pcu.PromoCodeId);

        builder.Property(pcu => pcu.DiscountApplied)
               .HasPrecision(18, 2);

    }
}
