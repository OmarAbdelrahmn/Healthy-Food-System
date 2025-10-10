
using Domain.Enums;

namespace Domain.Models.Entities;

public class PromoCode
{
    public Guid Id { get; set; }

    //public string? Code { get; set; }
    public DiscountType DiscountType { get; set; }
    //public decimal DiscountValue { get; set; }
    //public DateTime ExpiryDate { get; set; }
    //public bool IsActive { get; set; }
    public string? OwnerUserId { get; set; }
    public ICollection<PromoCodeUsage> Usages { get; set; } = new List<PromoCodeUsage>();

    ///////////////////////////
    public string Code { get; private set; }
    public string Description { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public decimal MinimumOrderAmount { get; private set; }

    public int MaxUsageCount { get; private set; }
    public int UsedCount { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidUntil { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<PromoCodeUsage> _usages = new();
    public IReadOnlyCollection<PromoCodeUsage> UsagesReadOnly => _usages.AsReadOnly();

    private PromoCode()
    { } // For EF Core

    public PromoCode(string code, string description, decimal discountAmount, decimal discountPercentage, decimal minimumOrderAmount, int maxUsageCount, DateTime validFrom, DateTime validUntil)
    {
        Code = code?.ToUpper() ?? throw new ArgumentNullException(nameof(code));
        Description = description;
        DiscountAmount = discountAmount;
        DiscountPercentage = discountPercentage;
        MinimumOrderAmount = minimumOrderAmount;
        MaxUsageCount = maxUsageCount;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        IsActive = true;
        UsedCount = 0;
    }

    public bool isValid()
    {
        var now = DateTime.UtcNow;
        return IsActive &&
            now >= ValidFrom &&
            now <= ValidUntil &&
            UsedCount < MaxUsageCount;
    }

    public decimal CalculateDiscount(decimal orderAmount)
    {
        if (orderAmount < MinimumOrderAmount)
            return 0;
        decimal discount = DiscountAmount;
        if (DiscountPercentage > 0)
        {
            var percentageDiscount = orderAmount * (DiscountPercentage / 100);
            discount = Math.Max(discount, DiscountAmount);
        }
        return Math.Min(discount, orderAmount);
    }

    public void MarkAsUsed(string userId, int orderId, decimal discountApplied)
    {
        UsedCount++;
        _usages.Add(new PromoCodeUsage(Id, userId, orderId, discountApplied));
    }



    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateUsageLimit(int maxUsageCount)
        => MaxUsageCount = maxUsageCount;

}
