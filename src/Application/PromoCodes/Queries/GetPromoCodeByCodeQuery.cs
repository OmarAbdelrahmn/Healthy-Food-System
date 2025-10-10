using Application.Interfaces;
using Domain.Models.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PromoCodes.Queries;
public class GetPromoCodeByCodeQuery : IRequest<PromoCode>
{
    public string Code { get; set; }
    public GetPromoCodeByCodeQuery(string code)
    {
        Code = code;
    }
}

public class GetPromoCodeByCodeQueryHandler : IRequestHandler<GetPromoCodeByCodeQuery, PromoCode>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetPromoCodeByCodeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<PromoCode> Handle(GetPromoCodeByCodeQuery request, CancellationToken cancellationToken)
    {
        var promoCode = await _unitOfWork.PromoCodes.GetByCodeAsync(request.Code, cancellationToken);
        if (promoCode == null)
        {
            throw new InvalidOperationException($"Promo code with code '{request.Code}' not found.");
        }
        return promoCode;
    }
}
