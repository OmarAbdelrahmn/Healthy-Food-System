
using Domain.Enums;

namespace Domain.Models.Entities;

public class PromoCode
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public DiscountType DiscountType { get; set; }
    //public decimal DiscountValue { get; set; }
    //public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public string? OwnerUserId { get; set; }
    public ICollection<PromoCodeUsage> Usages { get; set; } = new List<PromoCodeUsage>();

    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get;  set; }
    public int MaxUsageCount { get;  set; }
    public int CurrentUsageCount { get;  set; }
    public decimal MinimumOrderAmount { get; set; }

    private PromoCode() { }
    public PromoCode(string code, decimal discountAmount, decimal discountPercentage,
                     DateTime validFrom, DateTime validTo, int maxUsageCount, decimal minimumOrderAmount, string? ownerUserId = null)
    {
        Id = Guid.NewGuid();
        Code = code;
        DiscountAmount = discountAmount;
        DiscountPercentage = discountPercentage;
        ValidFrom = validFrom;
        ValidTo = validTo;
        MaxUsageCount = maxUsageCount;
        MinimumOrderAmount = minimumOrderAmount;
        OwnerUserId = ownerUserId;
        IsActive = true;
        CurrentUsageCount = 0;
    }

    public bool IsValid()
    {
        var now = DateTime.UtcNow;
        return IsActive &&
               now >= ValidFrom &&
               now <= ValidTo &&
               CurrentUsageCount < MaxUsageCount;
    }

    public decimal ApplyDiscount(decimal orderAmount)
    {
        if (!IsValid())
            throw new InvalidOperationException("Promo code is not valid.");
        if (MinimumOrderAmount > 0 && orderAmount < MinimumOrderAmount)
            throw new InvalidOperationException($"Order amount must be at least {MinimumOrderAmount} to use this promo code.");
        var discount = DiscountAmount;
        if (DiscountPercentage > 0)
        {
            discount += orderAmount * (DiscountPercentage / 100);
        }
        return Math.Min(discount, orderAmount);
    }

    public void MarkAsUsed()
    {
        if (CurrentUsageCount >= MaxUsageCount)
            throw new InvalidOperationException("Promo code usage limit reached.");
        CurrentUsageCount++;
        if (CurrentUsageCount >= MaxUsageCount)
            IsActive = false;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
