using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PomoCodeController : ControllerBase
{
    private readonly IPromoCodeService _pomoCodeService;
    public PomoCodeController(IPromoCodeService pomoCodeService)
    {
        _pomoCodeService = pomoCodeService;
    }
    [HttpPost]
    public async Task<ActionResult> CreatePromoCode([FromBody] CreatePromoCodeDto createDto)
    {
        try
        {
            var result = await _pomoCodeService.CreatePromoCodeAsync(createDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("apply")]
    public async Task<ActionResult<PromoCodeApplicationResultDto>> ApplyPromoCode([FromBody] ApplyPromoCodeDto applyDto)
    {
        var result = await _pomoCodeService.ApplyPromoCodeAsync(applyDto);
        if (result.IsValid)
        {
            return Ok(result);
        }
        return BadRequest(new { message = result.ErrorMessage });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PromoCodeDto>>> GetAllPromoCodes()
    {
        var result = await _pomoCodeService.GetAllPromoCodesAsync();
        return Ok(result);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<PromoCodeDto>> GetPromoCodeByCode(string code)
    {
        var result = await _pomoCodeService.GetPromoCodeByCodeAsync(code);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePromoCode(Guid id)
    {
        try
        {
            await _pomoCodeService.DeletePromoCodeAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
