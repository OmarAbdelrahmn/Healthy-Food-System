using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PromoCodes.Queries;
public class ValidatePromoCodeResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string Code { get; set; } = string.Empty;

    public string? ErrorType { get; set; }
    public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;
    public decimal OrderAmount { get; set; }

    public static ValidatePromoCodeResponse Success ( string code, decimal discountAmount, decimal finalAmount, string message = " Promo code applied successfully")
    {
        return new ValidatePromoCodeResponse
        {
            IsValid = true,
            Code = code,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            Message = message,
            ErrorType = null,
        };
    }

    public static ValidatePromoCodeResponse Failure ( string code, string message, string errorType)
    {
        return new ValidatePromoCodeResponse
        {
            IsValid = false,
            Code = code,
            DiscountAmount = 0,
            FinalAmount = 0,
            Message = $"Promo code '{code}' not found",
            ErrorType = "NotFound"
        };
    }

    public static ValidatePromoCodeResponse Expired(string code)
    {
        return new ValidatePromoCodeResponse
        {
            IsValid = false,
            Code = code,
            DiscountAmount = 0,
            FinalAmount = 0,
            Message = $"Promo code '{code}' has expired.",
            ErrorType = "Expired"
        };
    }

    public static ValidatePromoCodeResponse UsageLimitExceeded(string code)
    {
        return new ValidatePromoCodeResponse
        {
            IsValid = false,
            Code = code,
            DiscountAmount = 0,
            FinalAmount = 0,
            Message = $"Promo code '{code}' has exceeded its usage limit.",
            ErrorType = "UsageLimitExceeded"
        };
    }

    public static ValidatePromoCodeResponse MinimumOrderAmountNotMet(string code, decimal minimumOrderAmount)
    {
        return new ValidatePromoCodeResponse
        {
            IsValid = false,
            Code = code,
            DiscountAmount = 0,
            FinalAmount = 0,
            Message = $"Minimum order amount of {minimumOrderAmount:C} not met for promo code '{code}'.",
            ErrorType = "MinimumOrderAmountNotMet"
        };
    }

    public static ValidatePromoCodeResponse AlreadyUsed(string code)
    {
        return new ValidatePromoCodeResponse
        {
            IsValid = false,
            Code = code,
            DiscountAmount = 0,
            FinalAmount = 0,
            Message = $"Promo code '{code}' has already been used by this user.",
            ErrorType = "AlreadyUsed"
        };
    }

    public static ValidatePromoCodeResponse Inactive(string code)
    {
        return new ValidatePromoCodeResponse
        {
            IsValid = false,
            Code = code,
            DiscountAmount = 0,
            FinalAmount = 0,
            Message = $"Promo code '{code}' is inactive.",
            ErrorType = "Inactive"
        };
    }

    public static ValidatePromoCodeResponse Invalid(string code)
    {
        return new ValidatePromoCodeResponse
        {
            IsValid = false,
            Code = code,
            DiscountAmount = 0,
            FinalAmount = 0,
            Message = $"Promo code '{code}' not found.",
            ErrorType = "Invalid"
        };
    }

    public bool IsSuccess() => IsValid && string.IsNullOrEmpty(ErrorType);
    public bool IsFailure() => !IsValid && !string.IsNullOrEmpty(ErrorType);
    public override string ToString()
        => IsValid ? $"Success: {Message}, DiscountAmount: {DiscountAmount}, FinalAmount: {FinalAmount}, Code: {Code}"
                   : $"Failure: {Message}, Code: {Code}, ErrorType: {ErrorType}";

}

public class DetailedValidatePromoCodeResponse : ValidatePromoCodeResponse
{
    public decimal OriginalAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public int MaxUsageCount { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public static  DetailedValidatePromoCodeResponse fromPromoCode (
        ValidatePromoCodeResponse baseResponse,
        PromoCode promoCode,
        decimal originalAmount
    )
    {
        return new DetailedValidatePromoCodeResponse
        {
            IsValid = baseResponse.IsValid,
            Code = baseResponse.Code,
            DiscountAmount = baseResponse.DiscountAmount,
            FinalAmount = baseResponse.FinalAmount,
            Message = baseResponse.Message,
            ErrorType = baseResponse.ErrorType,
            ValidatedAt = baseResponse.ValidatedAt,
            OriginalAmount = originalAmount,
            DiscountPercentage = promoCode.DiscountPercentage,
            ValidFrom = promoCode.ValidFrom,
            ValidUntil = promoCode.ValidUntil,
            MaxUsageCount = promoCode.MaxUsageCount,
            MinimumOrderAmount = promoCode.MinimumOrderAmount,
            Description = promoCode.Description,
            IsActive = promoCode.IsActive
        };
    }
}
