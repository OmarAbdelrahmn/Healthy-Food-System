using Application.Interfaces.UnitOfWorkInterfaces;
using Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PromoCodes.Commands;
public record ApplyPromoCodeCommand : IRequest<PromoCodeValidationResult>
{
    public string Code { get; init; } = string.Empty;
    public decimal OrderAmount { get; init; }
    public string UserId { get; init; } = string.Empty;
    public int OrderId { get; init; }

    public ApplyPromoCodeCommand(string code, decimal orderAmount, string userId, int orderId)
    {
        Code = code;
        OrderAmount = orderAmount;
        UserId = userId;
        OrderId = orderId;
    }
}

public class ApplyPromoCodeCommandHandler : IRequestHandler<ApplyPromoCodeCommand, PromoCodeValidationResult>
{
    private readonly IUnitOfWork _unitOfWork;
    public ApplyPromoCodeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<PromoCodeValidationResult> Handle(ApplyPromoCodeCommand request, CancellationToken cancellationToken)
    {
        var promoCode = await _unitOfWork.PromoCodes.GetByCodeAsync(request.Code.ToUpper(), cancellationToken);

        if (promoCode == null)
        {
            return PromoCodeValidationResult.Invalid($"Promo code '{request.Code}' not found.");
        }
        var validationResult = promoCode.Validate(request.OrderAmount, request.UserId, request.OrderId);
        if (!validationResult.IsValid)
        {
            return validationResult;
        }

        if (request.OrderAmount < promoCode.MinimumOrderAmount)
        {
            return PromoCodeValidationResult.Invalid($"Order amount must be at least {promoCode.MinimumOrderAmount:C} to use this promo code.");
        }

        var userUsageCount = await _unitOfWork.PromoCodeUsages
            .CountUserUsageAsync(u => u.PromoCodeId == promoCode.Id && u.UserId == request.UserId, cancellationToken);

        if (userUsageCount >= 1)
            return PromoCodeValidationResult.Invalid("You have already used this promo code.");

        var discount = promoCode.CalculateDiscount(request.OrderAmount);
        var finalAmount = request.OrderAmount - discount;

        return PromoCodeValidationResult.Valid(discount, finalAmount, promoCode.Code);
    }
}