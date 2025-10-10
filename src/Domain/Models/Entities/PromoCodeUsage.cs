
namespace Domain.Models.Entities;

public class PromoCodeUsage
{
    public Guid Id { get; set; }
    public Guid PromoCodeId { get; set; }
    public string UserId { get; set; }
    public DateTime UsedAt { get; set; }
    public PromoCode PromoCode { get; private set; } = null!;
    public decimal DiscountApplied { get; private set; }
    //-----------------------------
    private PromoCodeUsage() { } // For EF Core
    public int OrderId { get; private set; }
    public PromoCodeUsage(Guid promoCodeId, string userId, int orderId, decimal discountApplied)
    {
        Id = Guid.NewGuid();
        PromoCodeId = promoCodeId;
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        OrderId = orderId;
        DiscountApplied = discountApplied;
        UsedAt = DateTime.UtcNow;
    }

}
