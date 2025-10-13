using Domain.Models.Entities;
using Infrastructure.Data;
using Infrastructure.Service.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.ingrediant;
internal class IngredientService(ApplicationDbContext dbContext) : IIngredientService
{
    private readonly ApplicationDbContext _context = dbContext;


    public async Task<Result<IngredientDetailDto>> GetByIdAsync(int id)
    {
        
        
            var ingredient = await _context.Ingredients
                .Include(i => i.Meals)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
            {
                return Result.Failure<IngredientDetailDto>(new Error($"Ingredient with ID {id} not found", "Ingredient with ID {id} not found" ,400));
            }

            var dto = new IngredientDetailDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                CaloriesPer100g = ingredient.CaloriesPer100g,
                ProteinPer100g = ingredient.ProteinPer100g,
                CarbsPer100g = ingredient.CarbsPer100g,
                FatsPer100g = ingredient.FatsPer100g,
                Meals = ingredient.Meals?.Select(m => new MealSummaryDto
                {
                    Id = m.Id,
                    Name = m.Name
                }).ToList() ?? new List<MealSummaryDto>()
            };

            return Result.Success(dto);
        
             
    }

    public async Task<Result<IEnumerable<IngredientListDto>>> GetAllAsync()
    {
        
            var ingredients = await _context.Ingredients
                .Include(i => i.Meals)
                .OrderBy(i => i.Name)
                .ToListAsync();

            var dtos = ingredients.Select(i => new IngredientListDto
            {
                Id = i.Id,
                Name = i.Name,
                CaloriesPer100g = i.CaloriesPer100g,
                MealCount = i.Meals?.Count ?? 0
            }).ToList();


            return Result.Success<IEnumerable<IngredientListDto>>(dtos);
        
    
    }

    public async Task<Result<IngredientDetailDto>> CreateAsync(IngredientCreateDto createDto)
    {
        
            var validationErrors = ValidateIngredient(createDto);
            if (validationErrors.Any())
            {
                return Result.Failure<IngredientDetailDto>(new Error(
                    "Validation failed", "this some thing wrong with this ingredient",400));
            }

            var nameExists = await _context.Ingredients
                .AnyAsync(i => i.Name.ToLower() == createDto.Name.ToLower());

            if (nameExists)
            {
                return Result.Failure<IngredientDetailDto>(new Error(
                    "name exists", "this ingredient is exists with this name", 400));
            }

            var ingredient = new Ingredient
            {
                Name = createDto.Name,
                CaloriesPer100g = createDto.CaloriesPer100g,
                ProteinPer100g = createDto.ProteinPer100g,
                CarbsPer100g = createDto.CarbsPer100g,
                FatsPer100g = createDto.FatsPer100g,
                Meals = new List<Meal>()
            };

            if (createDto.MealIds?.Any() == true)
            {
                var meals = await _context.Meals
                    .Where(m => createDto.MealIds.Contains(m.Id))
                    .ToListAsync();

                ingredient.Meals = meals;
            }

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            var result = await _context.Ingredients
                .Include(i => i.Meals)
                .FirstAsync(i => i.Id == ingredient.Id);

            var dto = MapToDetailDto(result);


            return Result.Success(dto);
       
    }

    public async Task<Result<IngredientDetailDto>> UpdateAsync(IngredientUpdateDto updateDto)
    {
        
            var existing = await _context.Ingredients
                .Include(i => i.Meals)
                .FirstOrDefaultAsync(i => i.Id == updateDto.Id);

            if (existing == null)
            {
                return Result.Failure<IngredientDetailDto>( new Error(
                    $"Ingredient with ID {updateDto.Id} not found","this ingredient is not exists",400));
            }

            var validationErrors = ValidateIngredient(updateDto);
            if (validationErrors.Any())
            {
                return Result.Failure<IngredientDetailDto>(new Error(
                     "Validation failed", "this some thing wrong with this ingredient", 400));
            }

            var nameExists = await _context.Ingredients
                .AnyAsync(i => i.Name.ToLower() == updateDto.Name.ToLower() && i.Id != updateDto.Id);

            if (nameExists)
            {
                return Result.Failure<IngredientDetailDto>(new Error(
                   $"Ingredient with ID {updateDto.Id} not found", "this ingredient is not exists", 400));
            }

            existing.Name = updateDto.Name;
            existing.CaloriesPer100g = updateDto.CaloriesPer100g;
            existing.ProteinPer100g = updateDto.ProteinPer100g;
            existing.CarbsPer100g = updateDto.CarbsPer100g;
            existing.FatsPer100g = updateDto.FatsPer100g;

            if (updateDto.MealIds != null)
            {
                existing.Meals.Clear();

                if (updateDto.MealIds.Any())
                {
                    var meals = await _context.Meals
                        .Where(m => updateDto.MealIds.Contains(m.Id))
                        .ToListAsync();

                    foreach (var meal in meals)
                    {
                        existing.Meals.Add(meal);
                    }
                }
            }

            await _context.SaveChangesAsync();

            var result = await _context.Ingredients
                .Include(i => i.Meals)
                .FirstAsync(i => i.Id == updateDto.Id);

            var dto = MapToDetailDto(result);


            return Result.Success(
                dto);
    
    }

    public async Task<Abstraction.Result> DeleteAsync(int id)
    {
        
            var ingredient = await _context.Ingredients
                .Include(i => i.Meals)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
            {
                return Abstraction.Result.Failure(new Abstraction.Error(
                    $"Ingredient not found", "this ingredient is not exists", 400)); 
            }

            if (ingredient.Meals?.Any() == true)
            {
                return Result.Failure( new Error(
                    $"Cannot delete ingredient. It is used in {ingredient.Meals.Count} meal(s)","cant be deleted",400));
            }

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();


            return Result.Success();
        }

    public async Task<Result<IEnumerable<IngredientListDto>>> SearchAsync(string searchTerm)
    {
       
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            var ingredients = await _context.Ingredients
                .Include(i => i.Meals)
                .Where(i => i.Name.Contains(searchTerm))
                .OrderBy(i => i.Name)
                .ToListAsync();

            var dtos = ingredients.Select(MapToListDto).ToList();

            return Result.Success<IEnumerable<IngredientListDto>>(dtos);
        
    }

    public async Task<Result<NutritionSummary>> GetNutritionSummaryAsync(int id, decimal servingSize)
    {
        
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
            {
                return Result.Failure<NutritionSummary>(new Error(
                    $"Ingredient with ID {id} not found","not fount",400));
            }

            var multiplier = servingSize / 100m;
            var summary = new NutritionSummary
            {
                ServingSize = servingSize,
                Calories = ingredient.CaloriesPer100g * multiplier,
                Protein = ingredient.ProteinPer100g * multiplier,
                Carbs = ingredient.CarbsPer100g * multiplier,
                Fats = ingredient.FatsPer100g * multiplier
            };

            return Result.Success(summary);
        
    }

    private List<string> ValidateIngredient(object dto)
    {
        var errors = new List<string>();

        var name = dto.GetType().GetProperty("Name")?.GetValue(dto) as string;
        var calories = (decimal)(dto.GetType().GetProperty("CaloriesPer100g")?.GetValue(dto) ?? 0m);
        var protein = (decimal)(dto.GetType().GetProperty("ProteinPer100g")?.GetValue(dto) ?? 0m);
        var carbs = (decimal)(dto.GetType().GetProperty("CarbsPer100g")?.GetValue(dto) ?? 0m);
        var fats = (decimal)(dto.GetType().GetProperty("FatsPer100g")?.GetValue(dto) ?? 0m);

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("Name is required");
        else if (name.Length > 200)
            errors.Add("Name cannot exceed 200 characters");

        if (calories < 0)
            errors.Add("Calories cannot be negative");
        else if (calories > 10000)
            errors.Add("Calories value seems unrealistic (max 10000)");

        if (protein < 0)
            errors.Add("Protein cannot be negative");
        else if (protein > 100)
            errors.Add("Protein cannot exceed 100g per 100g");

        if (carbs < 0)
            errors.Add("Carbs cannot be negative");
        else if (carbs > 100)
            errors.Add("Carbs cannot exceed 100g per 100g");

        if (fats < 0)
            errors.Add("Fats cannot be negative");
        else if (fats > 100)
            errors.Add("Fats cannot exceed 100g per 100g");

        if (protein + carbs + fats > 100)
            errors.Add("Total macros (protein + carbs + fats) cannot exceed 100g per 100g");

        return errors;
    }

    private IngredientDetailDto MapToDetailDto(Ingredient ingredient)
    {
        return new IngredientDetailDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            CaloriesPer100g = ingredient.CaloriesPer100g,
            ProteinPer100g = ingredient.ProteinPer100g,
            CarbsPer100g = ingredient.CarbsPer100g,
            FatsPer100g = ingredient.FatsPer100g,
            Meals = ingredient.Meals?.Select(m => new MealSummaryDto
            {
                Id = m.Id,
                Name = m.Name
            }).ToList() ?? new List<MealSummaryDto>()
        };
    }

    private IngredientListDto MapToListDto(Ingredient ingredient)
    {
        return new IngredientListDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            CaloriesPer100g = ingredient.CaloriesPer100g,
            MealCount = ingredient.Meals?.Count ?? 0
        };
    }
}

