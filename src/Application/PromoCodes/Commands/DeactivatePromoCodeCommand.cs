using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PromoCodes.Commands;
public class DeactivatePromoCodeCommand : IRequest
{
    public int PromoCodeId { get; init; }
    public DeactivatePromoCodeCommand(int promoCodeId)
    {
        PromoCodeId = promoCodeId;
    }
}

public class DeactivatePromoCodeCommandHandler : IRequestHandler<DeactivatePromoCodeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeactivatePromoCodeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(DeactivatePromoCodeCommand request, CancellationToken cancellationToken)
    {
        var promoCode = await _unitOfWork.PromoCodes.GetByIdAsync(request.PromoCodeId, cancellationToken);
        if (promoCode == null)
        {
            throw new InvalidOperationException($"Promo code with ID '{request.PromoCodeId}' not found.");
        }
        promoCode.Deactivate();
        await _unitOfWork.PromoCodes.UpdateAsync(promoCode, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

}