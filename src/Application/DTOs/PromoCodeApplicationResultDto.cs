using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;
public class PromoCodeApplicationResultDto
{
    public bool IsValid { get; set; }

    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? ErrorMessage { get; set; }
    public PromoCodeDto? PromoCode { get; set; }
}
