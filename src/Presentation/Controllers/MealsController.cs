using Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;
[Route("[controller]")]
[ApiController]
[Authorize]
public class MealsController(IMealService service) : ControllerBase
{
    private readonly IMealService service = service;


    [HttpPost("")]
    public async Task<IActionResult> CreateAsync( [FromBody] MealRequest request)
    {
        var userId = User.GetUserId();

        var result = await service.CreateAsync(userId!, request);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetByUserIdAsync()
    {
        var userId = User.GetUserId();
        var result = await service.GetByuserIdAsync(userId!);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await service.GetAsync();
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("user/meals")]
    public async Task<IActionResult> GetMealsByUserIdAsync()
    {
        var userId = User.GetUserId();
        var result = await service.GetMealsByuserIdAsync(userId!);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] oldMealRequest request)
    {
        var result = await service.UpdateAsync(id, request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPut("{MealId}/user")]
    public async Task<IActionResult> UpdateByUserIdAsync(int MealId , oldMealRequest request)
     {
         var userId = User.GetUserId();
         var result = await service.UpdateUserMealAsync(userId!,MealId, request);
         return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpDelete("{MealId}/user")]
    public async Task<IActionResult> DeleteByUserIdAsync(int MealId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await service.DeleteUserMealAsync(userId!,MealId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpDelete("user")]
    public async Task<IActionResult> DeleteAllByUserIdAsync(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await service.DeleteAllUserMealAsync(userId!, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


}




























public static class UserExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
