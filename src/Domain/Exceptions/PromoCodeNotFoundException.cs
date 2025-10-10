using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions;
public class PromoCodeNotFoundException : Exception
{
    public PromoCodeNotFoundException(string code)
        : base($"Promo code '{code}' not found.")
    {
    }

}
