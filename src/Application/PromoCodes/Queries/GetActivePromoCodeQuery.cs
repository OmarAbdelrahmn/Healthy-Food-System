using Application.Interfaces.UnitOfWorkInterfaces;
using AutoMapper;
using Domain.Models.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PromoCodes.Queries;
public record GetActivePromoCodeQuery : IRequest<List<PromoCode>>;

public class GetActivePromoCodeQueryHandler : IRequestHandler<GetActivePromoCodeQuery, List<PromoCode>>
{
    private readonly IUnitOfWork _unitOfWork;
    //private readonly IMapper _mapper;
    public GetActivePromoCodeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<PromoCode>> Handle(GetActivePromoCodeQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.PromoCodes.GetActivePromoCodesAsync(cancellationToken);
    }
}
