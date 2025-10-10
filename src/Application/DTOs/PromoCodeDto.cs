using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;
public class PromoCodeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int MaxUsageCount { get; set; }
    public int CurrentUsageCount { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public bool IsActive { get; set; }
    public string? OwnerUserId { get; set; }
    public bool IsValid { get; internal set; }
}
