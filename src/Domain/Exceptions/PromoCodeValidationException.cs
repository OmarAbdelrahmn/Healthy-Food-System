using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions;
public class PromoCodeValidationException : Exception
{
    public PromoCodeValidationException(string message)
        : base(message)
    {
    }
}
