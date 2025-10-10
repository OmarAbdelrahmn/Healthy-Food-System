using Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations;
public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.ToTable("PromoCodes");

        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pc => pc.Description)
            .HasMaxLength(500);

        builder.Property(pc => pc.DiscountAmount)
            .HasPrecision(18,2);

        builder.Property(pc => pc.DiscountPercentage)
            .HasPrecision(5 , 2);

        builder.Property(pc => pc.MinimumOrderAmount)
        .HasPrecision(18, 2);

        builder.Property(pc => pc.MaxUsageCount);

        builder.Property(pc => pc.ValidFrom).IsRequired();
        builder.Property(pc => pc.ValidUntil).IsRequired();
        builder.HasMany(pc => pc.Usages)
               .WithOne(u => u.PromoCode)
               .HasForeignKey(u => u.PromoCodeId);
    }
}
