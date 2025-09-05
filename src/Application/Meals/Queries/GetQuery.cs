using Application.Items.Queries.GetItem;
using Domain.Enums;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Meals.Queries.GetQuery;

namespace Application.Meals.Queries;
public class GetQuery : IRequest<ErrorOr<GetQueryResponse>>{

public record GetQueryResponse(
    Guid Id,
    Guid ItemId,
    string ItemName,
    string ItemNameAr,
    decimal Weight,
    decimal Price,
    MealType MealType,
    uint Quantity,
    DateTime CreatedAt
    );

}

