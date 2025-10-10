using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PromoCodeController : ControllerBase
{
    private readonly IPromoCodeService _pomoCodeService;
    private readonly ILogger<PromoCodeController> _logger;
    public PromoCodeController(IPromoCodeService pomoCodeService, ILogger<PromoCodeController> logger)
    {
        _pomoCodeService = pomoCodeService;
        _logger = logger;
    }
    [HttpPost]
    public async Task<ActionResult<PromoCodeDto>> CreatePromoCode([FromBody] CreatePromoCodeDto createDto)
    {
        try
        {
            var result = await _pomoCodeService.CreatePromoCodeAsync(createDto);
            return CreatedAtAction(nameof(GetPromoCodeByCode), new { code = result.Code }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating promo code");
            return StatusCode(500, new { message = "An error occurred while creating the promo code." });
        }
    }
    [HttpPost("validate")]
    public async Task<ActionResult<PromoCodeApplicationResultDto>> ValidatePromoCode([FromBody] ApplyPromoCodeDto applyDto)
    {
        var result = await _pomoCodeService.ValidationPromoCodeAsync(applyDto);
        if (result.IsValid)
        {
            return Ok(result);
        }
        return BadRequest(new { message = result.ErrorMessage });
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
    [HttpGet("{id}")]
    public async Task<ActionResult<PromoCodeDto>> GetPromoCodeById(Guid id)
    {
        var result = await _pomoCodeService.GetPromoCodeByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
    [HttpPatch("usage")]
    public async Task<ActionResult> UpdateUsage([FromBody] UpdatePromoCodeUsageDto updateDto)
    {
        try
        {
            await _pomoCodeService.UpdateUsageAsync(updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpPatch("deactivate/{id}")]
    public async Task<ActionResult> DeactivatePromoCode(Guid id)
    {
        try
        {
            await _pomoCodeService.DeactivatePromoCodeAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("exists/{code}")]
    public async Task<ActionResult<bool>> PromoCodeExists(string code)
    {
        var exists = await _pomoCodeService.PromoCodeExistsAsync(code);
        return Ok(exists);
    }

}
