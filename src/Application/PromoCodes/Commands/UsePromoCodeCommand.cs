using Application.Interfaces.UnitOfWorkInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PromoCodes.Commands;
public record UsePromoCodeCommand : IRequest<bool>
{
    public string Code { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public int OrderId { get; init; }
    public decimal OrderAmount { get; init; }
}

public class UsePromoCodeCommandHandler : IRequestHandler<UsePromoCodeCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UsePromoCodeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<bool> Handle(UsePromoCodeCommand request, CancellationToken cancellationToken)
    {
        var promoCode = await _unitOfWork.PromoCodes
            .GetByCodeAsync(request.Code, cancellationToken);
        if (promoCode == null)
        {
            throw new InvalidOperationException($"Promo code '{request.Code}' not found.");
        }
        var validationResult = promoCode.Validate(request.OrderAmount, request.UserId, request.OrderId);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(validationResult.Message);
        }
        var userUsageCount = await _unitOfWork.PromoCodeUsages
            .CountUserUsageAsync(u => u.PromoCodeId == promoCode.Id && u.UserId == request.UserId, cancellationToken);
        if (userUsageCount >= 1)
            throw new InvalidOperationException("You have already used this promo code.");
        var usage = new PromoCodeUsage
        {
            PromoCodeId = promoCode.Id,
            UserId = request.UserId,
            OrderId = request.OrderId,
            UsedAt = DateTime.UtcNow
        };
        await _unitOfWork.PromoCodeUsages.AddAsync(usage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
