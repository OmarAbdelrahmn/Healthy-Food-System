
using Infrastructure.Service.Abstraction;

namespace Infrastructure.Service;
public interface IMealService
{
    Task<Result<MealResponse>> CreateAsync(string userId ,MealRequest request);
    Task<Result<MealResponse>> GetByIdAsync(int Id );
    Task<Result<MealResponse>> GetByuserIdAsync(string UserId , int id);
    Task<Result<IEnumerable<MealResponse>>> GetMealsByuserIdAsync(string UserId);
    Task<Result<IEnumerable<MealResponse>>> GetAsync();
    Task<Result<oldMealResponse>> UpdateAsync(int MealId, oldMealRequest request);
    Task<Result<oldMealResponse>> UpdateUserMealAsync(string UserId, int MealId, oldMealRequest request);
    Task<Result> DeleteAsync(int Id, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserMealAsync(string UserId,int mealId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAllUserMealAsync(string UserId, CancellationToken cancellationToken = default);

}
