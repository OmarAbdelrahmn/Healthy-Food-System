using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;
public class CreatePromoCodeDto
{
    public string Code { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int MaxUsageCount { get; set; }
    public decimal MinimumOrderAmount { get; set; }

}
