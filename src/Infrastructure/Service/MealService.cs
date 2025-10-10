

using Domain.Models.Entities;
using Infrastructure.Data;
using Infrastructure.Service.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Service;
public class MealService(ApplicationDbContext dbContext) : IMealService
{
    private readonly ApplicationDbContext dbContext = dbContext;

    public async Task<Result<MealResponse>> CreateAsync(string userId, MealRequest request)
    {
        var MealIsExi = await dbContext.Meals.AnyAsync(c => c.Name == request.Name);

        if (MealIsExi)
            Result.Failure(new Error("alreadyexists", "meal with this name is already exists", 400));
    
        var meal = new Meal {
            Name = request.Name,
            ImageUrl = request.ImageUrl,
            Description = request.Description,
            FixedCalories = request.FixedCalories,
            FixedProtein = request.FixedProtein,
            FixedCarbs = request.FixedCarbs,
            FixedFats = request.FixedFats,
            MealType = request.MealType,
            AcceptCarb = request.AcceptCarb,
            SubcategoryId = request.SubcategoryId,
            IngredientId = request.IngredientId,
            DefaultQuantityGrams = request.DefaultQuantityGrams,
            UserId = userId
        };

        await dbContext.Meals.AddAsync(meal);

        await dbContext.SaveChangesAsync();

        var response = new MealResponse(
            meal.Id,
            meal.Name,
            meal.ImageUrl,
            meal.Description,
            meal.FixedCalories,
            meal.FixedProtein,
            meal.FixedCarbs,
            meal.FixedFats,
            meal.MealType,
            meal.AcceptCarb,
            meal.SubcategoryId,
            meal.IngredientId,
            meal.DefaultQuantityGrams
            ,meal.UserId!
            );


        return Result.Success(response);


    }

    public async Task<Result> DeleteAllUserMealAsync(string UserId, CancellationToken cancellationToken = default)
    {
        var meals = await dbContext.Meals.Where(m => m.UserId == UserId).ToListAsync(cancellationToken: cancellationToken);

        if (meals.Count == 0)
            return Result.Failure(new Error("NotFound", "meals for this user not found", StatusCodes.Status404NotFound));

        dbContext.Meals.RemoveRange(meals);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int Id, CancellationToken cancellationToken = default)
    {
        var meal =await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == Id, cancellationToken: cancellationToken);

        if (meal == null)
            return Result.Failure(new Error("NotFound", "meal with this id not found", StatusCodes.Status404NotFound));

        dbContext.Meals.Remove(meal);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteUserMealAsync(string UserId,int mealId, CancellationToken cancellationToken = default)
    {
        var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.UserId == UserId && m.Id == mealId, cancellationToken: cancellationToken);

        if (meal == null)
            return Result.Failure(new Error("NotFound", "meal with this user id not found", StatusCodes.Status404NotFound));

        dbContext.Meals.Remove(meal);
        await dbContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<IEnumerable<MealResponse>>> GetAsync()
    {
        var meals = await dbContext.Meals.ToListAsync();

        var response = meals.Select(meal => new MealResponse(
            meal.Id,
            meal.Name,
            meal.ImageUrl,
            meal.Description,
            meal.FixedCalories,
            meal.FixedProtein,
            meal.FixedCarbs,
            meal.FixedFats,
            meal.MealType,
            meal.AcceptCarb,
            meal.SubcategoryId,
            meal.IngredientId,
            meal.DefaultQuantityGrams,
            meal.UserId!
            )).ToList();

        return Result.Success(response.AsEnumerable());
    }

    public async Task<Result<MealResponse>> GetByIdAsync(int Id)
    {
        var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == Id);

        if (meal == null)
            return Result.Failure<MealResponse>(new Error("NotFound", "meal with this id not found", StatusCodes.Status404NotFound));

        var response = new MealResponse(
            meal.Id,
            meal.Name,
            meal.ImageUrl,
            meal.Description,
            meal.FixedCalories,
            meal.FixedProtein,
            meal.FixedCarbs,
            meal.FixedFats,
            meal.MealType,
            meal.AcceptCarb,
            meal.SubcategoryId,
            meal.IngredientId,
            meal.DefaultQuantityGrams
            ,meal.UserId!
            );
        return Result.Success(response);

    }

    public async Task<Result<MealResponse>> GetByuserIdAsync(string UserId)
    {
        var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.UserId == UserId);
        
        if (meal == null)
            return Result.Failure<MealResponse>(new Error("NotFound", "meal with this user id not found", StatusCodes.Status404NotFound));

        var response = new MealResponse(
            meal.Id,
            meal.Name,
            meal.ImageUrl,
            meal.Description,
            meal.FixedCalories,
            meal.FixedProtein,
            meal.FixedCarbs,
            meal.FixedFats,
            meal.MealType,
            meal.AcceptCarb,
            meal.SubcategoryId,
            meal.IngredientId,
            meal.DefaultQuantityGrams
            ,meal.UserId!
            );

        return Result.Success(response);

    }

    public async Task<Result<IEnumerable<MealResponse>>> GetMealsByuserIdAsync(string UserId)
    {
        var meals =await dbContext.Meals.Where(m => m.UserId == UserId).ToListAsync();

        if (meals.Count == 0)
            return Result.Failure<IEnumerable<MealResponse>>(new Error("NotFound", "meals for this user not found", StatusCodes.Status404NotFound));
        
        var response = meals.Select(meal => new MealResponse(
            meal.Id,
            meal.Name,
            meal.ImageUrl,
            meal.Description,
            meal.FixedCalories,
            meal.FixedProtein,
            meal.FixedCarbs,
            meal.FixedFats,
            meal.MealType,
            meal.AcceptCarb,
            meal.SubcategoryId,
            meal.IngredientId,
            meal.DefaultQuantityGrams,
            meal.UserId!
            )).ToList();
        return Result.Success(response.AsEnumerable());

    }

    public async Task<Result<oldMealResponse>> UpdateAsync(int MealId, oldMealRequest request)
    {
        var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.Id == MealId);

        if (meal == null)
            return Result.Failure<oldMealResponse>(new Error("NotFound", "meal with this id not found", StatusCodes.Status404NotFound));

        meal.Name = request.Name;
        meal.ImageUrl = request.ImageUrl;
        meal.Description = request.Description;
        meal.FixedCalories = request.FixedCalories;
        meal.FixedProtein = request.FixedProtein;
        meal.FixedCarbs = request.FixedCarbs;
        meal.FixedFats = request.FixedFats;
        meal.MealType = request.MealType;
        meal.AcceptCarb = request.AcceptCarb;
        meal.SubcategoryId = request.SubcategoryId;
        meal.IngredientId = request.IngredientId;
        meal.DefaultQuantityGrams = request.DefaultQuantityGrams;
        meal.NewQuantityGrams = request.NewQuantityGram;
        meal.NewCalories = request.NewCalories;
        meal.NewProtein = request.NewProtein;
        meal.NewCarbs = request.NewCarbs;
        meal.NewFats = request.NewFats;

        dbContext.Update(meal);
        await dbContext.SaveChangesAsync();

        var response = new oldMealResponse(
            meal.Id,
            meal.Name,
            meal.ImageUrl,
            meal.Description,
            meal.FixedCalories,
            meal.FixedProtein,
            meal.FixedCarbs,
            meal.FixedFats,
            meal.MealType,
            meal.AcceptCarb,
            meal.SubcategoryId,
            meal.IngredientId,
            meal.DefaultQuantityGrams,
            meal.NewQuantityGrams,
            meal.NewCalories,
            meal.NewProtein,
            meal.NewCarbs,
            meal.NewFats,
            meal.UserId!
            );

        return Result.Success(response);


    }

    public async Task<Result<oldMealResponse>> UpdateUserMealAsync(string UserId, int MealId , oldMealRequest request)
    {
        var meal = await dbContext.Meals.FirstOrDefaultAsync(m => m.UserId == UserId && m.Id ==MealId);

        if (meal == null)
            return Result.Failure<oldMealResponse>(new Error("NotFound", "meal for this usernot found", StatusCodes.Status404NotFound));

        meal.Name = request.Name;
        meal.ImageUrl = request.ImageUrl;
        meal.Description = request.Description;
        meal.FixedCalories = request.FixedCalories;
        meal.FixedProtein = request.FixedProtein;
        meal.FixedCarbs = request.FixedCarbs;
        meal.FixedFats = request.FixedFats;
        meal.MealType = request.MealType;
        meal.AcceptCarb = request.AcceptCarb;
        meal.SubcategoryId = request.SubcategoryId;
        meal.IngredientId = request.IngredientId;
        meal.DefaultQuantityGrams = request.DefaultQuantityGrams;
        meal.NewQuantityGrams = request.NewQuantityGram;
        meal.NewCalories = request.NewCalories;
        meal.NewProtein = request.NewProtein;
        meal.NewCarbs = request.NewCarbs;
        meal.NewFats = request.NewFats;

        dbContext.Update(meal);
        await dbContext.SaveChangesAsync();

        var response = new oldMealResponse(
            meal.Id,
            meal.Name,
            meal.ImageUrl,
            meal.Description,
            meal.FixedCalories,
            meal.FixedProtein,
            meal.FixedCarbs,
            meal.FixedFats,
            meal.MealType,
            meal.AcceptCarb,
            meal.SubcategoryId,
            meal.IngredientId,
            meal.DefaultQuantityGrams,
            meal.NewQuantityGrams,
            meal.NewCalories,
            meal.NewProtein,
            meal.NewCarbs,
            meal.NewFats
            ,meal.UserId!
            );

        return Result.Success(response);
    }
}
