using Infrastructure.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.ingrediant;
public interface IIngredientService
{
    Task<Result<IngredientDetailDto>> GetByIdAsync(int id);
    Task<Result<IEnumerable<IngredientListDto>>> GetAllAsync();
    Task<Result<IngredientDetailDto>> CreateAsync(IngredientCreateDto createDto);
    Task<Result<IngredientDetailDto>> UpdateAsync(IngredientUpdateDto updateDto);
    Task<Result> DeleteAsync(int id);
    Task<Result<IEnumerable<IngredientListDto>>> SearchAsync(string searchTerm);
    Task<Result<NutritionSummary>> GetNutritionSummaryAsync(int id, decimal servingSize);
}
