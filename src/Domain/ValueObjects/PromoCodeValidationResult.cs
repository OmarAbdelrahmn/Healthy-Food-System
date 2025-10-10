using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects;
public class PromoCodeValidationResult
{
    public bool IsValid { get; }
    public string Message { get; }
    public decimal DiscountAmount { get; }
    public decimal FinalAmount { get; }
    public string Code { get; }

    private PromoCodeValidationResult(bool isValid, string message, decimal discountAmount, decimal finalAmount, string code)
    {
        IsValid = isValid;
        Message = message;
        DiscountAmount = discountAmount;
        FinalAmount = finalAmount;
        Code = code;
    }

    public static PromoCodeValidationResult Valid (decimal discountAmount, decimal finalAmount, string code)
        => new (true, "Promo code applied successfully.", discountAmount, finalAmount, code);

    public static PromoCodeValidationResult Invalid(string message)
        => new (false, message, 0, 0, string.Empty);
}
