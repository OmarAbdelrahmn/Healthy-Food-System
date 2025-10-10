using Domain.Models.Entities;
using Domain.Models.Identity;
using Infrastructure.Migrations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Meal>()
            .HasOne(m => m.User)
            .WithMany(u => u.meals)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ApplyConfigurationsFromAssembly
            (typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);

    }
}
