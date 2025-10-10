

using Domain.Enums;

namespace Infrastructure.Service;
public record MealResponse
(
    int id,
        string Name,
    string ImageUrl,
    string Description,
    decimal? FixedCalories,
    decimal? FixedProtein,
    decimal? FixedCarbs,
    decimal? FixedFats,
    MealType? MealType,
    bool AcceptCarb,
    int SubcategoryId,
    int? IngredientId,
    decimal? DefaultQuantityGrams,
    string userId
    );
public record oldMealResponse
(
    int id,
        string Name,
    string ImageUrl,
    string Description,
    decimal? FixedCalories,
    decimal? FixedProtein,
    decimal? FixedCarbs,
    decimal? FixedFats,
    MealType? MealType,
    bool AcceptCarb,
    int SubcategoryId,
    int? IngredientId,
    decimal? DefaultQuantityGrams,
    decimal? NewQuantityGram,
 decimal? NewCalories,
 decimal? NewProtein,
 decimal? NewCarbs,
 decimal? NewFats, string userId

    );
public record oldMealRequest
(
        string Name,
    string ImageUrl,
    string Description,
    decimal? FixedCalories,
    decimal? FixedProtein,
    decimal? FixedCarbs,
    decimal? FixedFats,
    MealType? MealType,
    bool AcceptCarb,
    int SubcategoryId,
    int? IngredientId,
    decimal? DefaultQuantityGrams,
    decimal? NewQuantityGram,
 decimal? NewCalories,
 decimal? NewProtein,
 decimal? NewCarbs,
 decimal? NewFats
    );
