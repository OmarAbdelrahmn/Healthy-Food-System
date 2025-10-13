
using Infrastructure.Service.ingrediant;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class IngredientController(IIngredientService service) : Controller
{
    private readonly IIngredientService _service = service;


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IngredientCreateDto createDto)
    {
        var result = await _service.CreateAsync(createDto);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }

    [HttpPut("")]

    public async Task<IActionResult> Update( [FromBody] IngredientUpdateDto updateDto)
    {

        var result = await _service.UpdateAsync(updateDto);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }

    [HttpDelete("{id}")]

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        return result.IsSuccess ? Ok() : result.ToProblem();

    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        var result = await _service.SearchAsync(term);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id}/nutrition")]

    public async Task<IActionResult> GetNutritionSummary(int id, [FromQuery] decimal servingSize = 100)
    {
        var result = await _service.GetNutritionSummaryAsync(id, servingSize);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }

}
