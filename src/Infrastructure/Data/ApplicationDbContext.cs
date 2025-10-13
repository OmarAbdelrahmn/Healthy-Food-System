using Domain.Models.Entities;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderMeal> OrderMeals { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subcategory> Subcategories { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<PromoCodeUsage> PromoCodeUsages { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<PlanCategory> PlanCategories { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<PaymentLog> PaymentLogs { get; set; }
    public DbSet<ReferralCode> ReferralCodes { get; set; }
    public DbSet<UserPrefernce> UserPrefernces { get; set; }
    public DbSet<SubscriptionCategory> SubscriptionCategories { get; set; }
    public DbSet<SupportTicket> SupportTickets { get; set; }
    public DbSet<SupportMessage> SupportMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<PromoCode>(entity =>
        {
            entity.HasIndex(pc => pc.Id);
            entity.Property(pc => pc.Code)
                    .IsRequired()
                    .HasMaxLength(50);
            entity.Property(pc => pc.DiscountAmount)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);
            entity.Property(pc => pc.DiscountPercentage)
                    .HasColumnType("decimal(5,2)")
                    .HasDefaultValue(0);
            entity.Property(pc => pc.ValidFrom)
                    .IsRequired();
            entity.Property(pc => pc.ValidTo)
                    .IsRequired();
            entity.Property(pc => pc.MaxUsageCount)
                    .IsRequired()
                    .HasDefaultValue(1);
            entity.Property(pc => pc.CurrentUsageCount)
                    .IsRequired()
                    .HasDefaultValue(0);
            entity.Property(pc => pc.MinimumOrderAmount)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

        });
    }
}
