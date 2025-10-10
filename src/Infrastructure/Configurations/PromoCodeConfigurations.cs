using Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations;
public class PromoCodeConfigurations : IEntityTypeConfiguration<PromoCode>
{

    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.HasKey(pc => pc.Id);
        builder.Property(pc => pc.Code)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(pc => pc.Code)
            .IsUnique();
        builder.Property(pc => pc.DiscountAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.Property(pc => pc.DiscountPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");
        builder.Property(pc => pc.ValidFrom)
            .IsRequired();
        builder.Property(pc => pc.ValidTo)
            .IsRequired();
        builder.Property(pc => pc.MaxUsageCount)
            .IsRequired();
        builder.Property(pc => pc.CurrentUsageCount)
            .IsRequired();
        builder.Property(pc => pc.MinimumOrderAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.Property(pc => pc.IsActive)
            .IsRequired();
        builder.Property(pc => pc.OwnerUserId)
            .HasMaxLength(450);
        builder.HasMany(pc => pc.Usages)
            .WithOne(u => u.PromoCode)
            .HasForeignKey(u => u.PromoCodeId);
        builder.HasIndex(pc => pc.Code)
            .IsUnique();
    }
}
