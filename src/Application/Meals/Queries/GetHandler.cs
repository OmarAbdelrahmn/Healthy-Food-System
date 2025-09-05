using Application.Interfaces.UnitOfWorkInterfaces;
using Application.Items.Queries.GetItem;
using Domain.DErrors;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Meals.Queries.GetQuery;

namespace Application.Meals.Queries;
internal class GetHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetQuery, ErrorOr<GetQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<GetQueryResponse>> Handle(
        GetQuery request,
        CancellationToken cancellationToken
    )
    {
        var meal = await _unitOfWork
            .Meals.GetQueryable()
            .AsNoTracking()
            .Include(m => m.Item)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);

        if (meal == null)
        {
            return DomainErrors.Meals.MealNotFound(request.Id);
        }

        return new GetQueryResponse(
            meal.Id,
            meal.ItemId,
            meal.Item.Name,
            meal.Item.NameAr,
            meal.Weight,
            meal.Price,
            meal.MealType,
            meal.Quantity,
            meal.CreatedAt
        );
    }
}

