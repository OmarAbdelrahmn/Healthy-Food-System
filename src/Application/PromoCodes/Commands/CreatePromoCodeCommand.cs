using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PromoCodes.Commands;
public record CreatePromoCodeCommand : IRequest<int>
{
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public decimal DiscountAmount { get; init; }
    public decimal DiscountPercentage { get; init; }

    public decimal MinimumOrderAmount { get; init; }
    public int maxUsageCount { get; init; }

    public DateTime ValidFrom { get; init; }
    public DateTime ValidUntil { get; init; }

    public CreatePromoCodeCommand ( string code, string description, decimal discountAmount, decimal discountPercentage, decimal minimumOrderAmount, int maxUsageCount, DateTime validFrom, DateTime validUntil)
    {
        Code = code;
        Description = description;
        DiscountAmount = discountAmount;
        DiscountPercentage = discountPercentage;
        MinimumOrderAmount = minimumOrderAmount;
        this.maxUsageCount = maxUsageCount;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
    }
}
