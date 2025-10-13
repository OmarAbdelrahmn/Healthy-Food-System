using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.ingrediant;
public class IngredientCreateDto
{
    public string Name { get; set; }
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public decimal FatsPer100g { get; set; }
    public List<int> MealIds { get; set; } = new List<int>();
}

public class IngredientUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public decimal FatsPer100g { get; set; }
    public List<int> MealIds { get; set; } = new List<int>();
}


public class IngredientDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public decimal FatsPer100g { get; set; }
    public List<MealSummaryDto> Meals { get; set; } = new List<MealSummaryDto>();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public decimal TotalMacros => ProteinPer100g + CarbsPer100g + FatsPer100g;
    public decimal ProteinPercentage => TotalMacros > 0 ? (ProteinPer100g / TotalMacros) * 100 : 0;
    public decimal CarbsPercentage => TotalMacros > 0 ? (CarbsPer100g / TotalMacros) * 100 : 0;
    public decimal FatsPercentage => TotalMacros > 0 ? (FatsPer100g / TotalMacros) * 100 : 0;
}


public class IngredientListDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal CaloriesPer100g { get; set; }
    public int MealCount { get; set; }
}

public class MealSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class NutritionSummary
{
    public decimal ServingSize { get; set; }
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fats { get; set; }
}
