using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects;
public class ValidatePromoCodeResult
{
    public bool IsValid { get; }
    public string Message { get; }
    public decimal DiscountAmount { get; }
    public decimal FinalAmount { get; }
    public string Code { get; }
    public PromoCodeValidationResult? Error { get; }

    private ValidatePromoCodeResult(bool isValid, string message, decimal discountAmount, decimal finalAmount, string code, PromoCodeValidationResult? error = null)
    {
        IsValid = isValid;
        Message = message;
        DiscountAmount = discountAmount;
        FinalAmount = finalAmount;
        Code = code;
        Error = error;
    }

    public static ValidatePromoCodeResult Success (decimal discountAmount, decimal finalAmount, string code)
        => new(true, "Promo code applied successfully.", discountAmount, finalAmount, code);

    public static ValidatePromoCodeResult NotFound(string code)
    {
        return new ValidatePromoCodeResult(
            isValid: false,
            message: $"Promo code '{code}' not found.",
            discountAmount: 0,
            finalAmount: 0,
            code: code,
            error: PromoCodeValidationResult.Invalid($"Promo code '{code}' not found.")
        );
    }

    public static ValidatePromoCodeResult Expired(string code)
    {
        return new ValidatePromoCodeResult(
            isValid: false,
            message: $"Promo code '{code}' has expired.",
            discountAmount: 0,
            finalAmount: 0,
            code: code,
            error: PromoCodeValidationResult.Invalid($"Promo code '{code}' has expired.")
        );
    }

    public static ValidatePromoCodeResult UsageLimitExceeded(string code)
    {
        return new ValidatePromoCodeResult(
            isValid: false,
            message: $"Promo code '{code}' has exceeded its usage limit.",
            discountAmount: 0,
            finalAmount: 0,
            code: code,
            error: PromoCodeValidationResult.Invalid($"Promo code '{code}' has exceeded its usage limit.")
        );
    }

    public static ValidatePromoCodeResult Inactive(string code)
    {
        return new ValidatePromoCodeResult(
            isValid: false,
            message: $"Promo code '{code}' is inactive.",
            discountAmount: 0,
            finalAmount: 0,
            code: code,
            error: PromoCodeValidationResult.Invalid($"Promo code '{code}' is inactuve.")
        );
    }

    public static ValidatePromoCodeResult MiniMumOrderAmountNotMet(string code, decimal minimumOrderAmount)
    {
        return new ValidatePromoCodeResult(
            isValid: false,
            message: $"Promo code '{code}' requires a minimum order amount of {minimumOrderAmount:C}.",
            discountAmount: 0,
            finalAmount: 0,
            code: code,
            error: PromoCodeValidationResult.Invalid($"Promo code '{code}' requires a minimum order amount of {minimumOrderAmount:C}.")
        );
    }

    public static ValidatePromoCodeResult AlreadyUsed (string code)
    {
        return new ValidatePromoCodeResult(
            isValid: false,
            message: $"Promo code '{code}' has already been used by this user.",
            discountAmount: 0,
            finalAmount: 0,
            code: code,
            error: PromoCodeValidationResult.Invalid($"Promo code '{code}' has already been used by this user.")
        );
    }

    public static ValidatePromoCodeResult Invalid (string message, string code)
        => new (false, message, 0, 0, code, PromoCodeValidationResult.Invalid(message));

    public bool IsSuccess() => IsValid && Error == null;

    public bool IsFailure() => !IsValid && Error != null;

    public override string ToString()
        => IsValid ? $"Success: {Message}, DiscountAmount: {DiscountAmount}, FinalAmount: {FinalAmount}, Code: {Code}" 
                   : $"Failure: {Message}, Code: {Code}, Error: {Error?.Message}";

    public enum PromoCodeValidationError
    {
        None = 0,
        NotFound = 1,
        Expired = 2,
        Inactive = 3,
        UsageLimitExceeded = 4,
        MinimumOrderAmountNotMet = 5,
        AlreadyUsed = 6,
        Invalid = 7
    }
}
