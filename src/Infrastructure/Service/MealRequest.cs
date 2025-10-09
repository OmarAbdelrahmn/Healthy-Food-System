
using Domain.Enums;

namespace Infrastructure.Service;
public record MealRequest
(
    string Name,
    string ImageUrl,
    string Description ,
    decimal? FixedCalories ,
    decimal FixedProtein ,
    decimal? FixedCarbs ,
    decimal? FixedFats,
    MealType? MealType,
    bool AcceptCarb,
    int SubcategoryId,
    int? IngredientId,
    decimal? DefaultQuantityGrams

);