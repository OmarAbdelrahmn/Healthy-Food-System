using Application.Interfaces.UnitOfWorkInterfaces;
using Domain.Models.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PromoCodes.Queries;
public record GetPromoCodeByIdQuery: IRequest<PromoCode>
{
    public int Id { get; set; }
    public GetPromoCodeByIdQuery(int id)
    {
        Id = id;
    }
}

public class GetPromoCodeByIdQueryHandler : IRequestHandler<GetPromoCodeByIdQuery, PromoCode>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetPromoCodeByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<PromoCode> Handle(GetPromoCodeByIdQuery request, CancellationToken cancellationToken)
    {
        var promoCode = await _unitOfWork.PromoCodes.GetByIdAsync(request.Id, cancellationToken);
        if (promoCode == null)
        {
            throw new InvalidOperationException($"Promo code with ID '{request.Id}' not found.");
        }
        return promoCode;
    }
}
