using Application.PromoCodes.Commands;
using Application.PromoCodes.Queries;
using Domain.Enums;
using Domain.Models.Entities;
using Domain.ValueObjects;
using Google.Apis.Admin.Directory.directory_v1.Data;
using ICSharpCode.Decompiler.CSharp.Syntax;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Presentation.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PromoCodesController : ControllerBase
{
    private readonly ISender _sender;
    public PromoCodesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePromoCode([FromBody] CreatePromoCodeCommand request)
    {
        var command = new CreatePromoCodeCommand(
            request.Code,
            request.Description,
            request.DiscountAmount,
            request.DiscountPercentage,
            request.MinimumOrderAmount,
            request.maxUsageCount,
            request.ValidFrom,
            request.ValidUntil
        );
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPost("validate")]

    public async Task<ActionResult<ValidatePromoCodeResponse>> Validate([FromBody] ValidatePromoCodeResponse request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var command = new ApplyPromoCodeCommand(request.Code, request.OrderAmount, userId ?? string.Empty);
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPost("use")]
    public async Task<ActionResult> Use([FromBody] UsePromoCodeCommand request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var command = new UsePromoCodeCommand(request.Code, userId ?? string.Empty, request.OrderId, request.OrderAmount);
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}/deactivate)"]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Deactivate(int id)
    {
        var command = new DeactivatePromoCodeCommand(id);
        await _sender.Send(command);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<List<PromoCode>>> GetActive()
    {
        var query = new GetActivePromoCodeQuery();
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PromoCode>> GetById(int id)
    {
        var query = new GetPromoCodeByIdQuery(id);
        var result = await _sender.Send(query);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

}

public class  CreatePromoCodeRequest
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Code { get; set; } = string.Empty;
    [StringLength(100)]
    public string Description { get; set; } = string.Empty;
    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }
    [Range(0, 100)]
    public decimal DiscountPercentage { get; set; }
    [Range(0, double.MaxValue)]
    public decimal MinimumOrderAmount { get; set; }
    [Range(1, int.MaxValue)]
    public int MaxUsageCount { get; set; }
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
    public DateTime ValidUntil { get; set; } = DateTime.UtcNow.AddMonths(1);
}

public class ValidatePromoCodeRequest
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Code { get; set; } = string.Empty;
    [Range(0.01, double.MaxValue)]
    public decimal OrderAmount { get; set; }

    [Range(1, double.MaxValue)]
    public int OrderId { get; set; }
}