using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;
public class ApplyPromoCodeDto
{
    public string Code { get; set; } = string.Empty;
    public decimal OrderAmount { get; set; }
}
