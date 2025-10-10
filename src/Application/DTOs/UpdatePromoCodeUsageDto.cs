using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;
public class UpdatePromoCodeUsageDto
{
    public string Code { get; set; } = string.Empty;
    public bool MarkAsUsed { get; set; } = true;
}
