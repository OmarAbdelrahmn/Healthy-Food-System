using Application.Interfaces.UnitOfWorkInterfaces;
using Domain.Models.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PromoCodes.Commands;
public class CreatePromoCodeHandler : IRequestHandler<CreatePromoCodeCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreatePromoCodeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
    {
        var existingCode = await _unitOfWork.PromoCodes.GetByCodeAsync(request.Code, cancellationToken);
        if (existingCode != null)
        {
            throw new InvalidOperationException($"Promo code '{request.Code}' already exists.");
        }
        var promoCode = new PromoCode(
            request.Code,
            request.Description,
            request.DiscountAmount,
            request.DiscountPercentage,
            request.MinimumOrderAmount,
            request.maxUsageCount,
            request.ValidFrom,
            request.ValidUntil
        );
        await _unitOfWork.PromoCodes.AddAsync(promoCode, cancellationToken);
        return await _unitOfWork.SaveChangesAsync(cancellationToken);

        return promoCode.Id;
    }
}
