using Infrastructure.Service.subscri;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;
[Route("[controller]")]
[ApiController]
public class SubscriptionController(ISubscriptionService subscriptionService) : ControllerBase
{


    private readonly ISubscriptionService _subscriptionService = subscriptionService;

    [HttpGet("statistics")]
    public async Task<ActionResult<SubscriptionStatisticsDto>> GetStatistics()
    {
        try
        {
            var stats = await _subscriptionService.GetSubscriptionStatisticsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving statistics", error = ex.Message });
        }
    }

    [HttpGet("customers")]
    public async Task<ActionResult<PagedResult<CustomerDetailsDtos>>> GetCustomers([FromQuery] CustomerFilterDto filter)
    {
        try
        {
            var customers = await _subscriptionService.GetCustomersAsync(filter);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving customers", error = ex.Message });
        }
    }

    [HttpGet("customers/{id}")]
    public async Task<ActionResult<CustomerDetailsDtos>> GetCustomerById(Guid id)
    {
        try
        {
            var customer = await _subscriptionService.GetCustomerByIdAsync(id);

            if (customer == null)
                return NotFound(new { message = "Customer not found" });

            return Ok(customer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving customer", error = ex.Message });
        }
    }

    [HttpPost("customers")]
    public async Task<ActionResult<CustomerDetailsDtos>> AddCustomer([FromBody] CreateCustomerDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _subscriptionService.AddCustomerAsync(dto);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error creating customer", error = ex.Message });
        }
    }

    [HttpPut("customers/{id}")]
    public async Task<ActionResult> UpdateCustomer(Guid id, [FromBody] CreateCustomerDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _subscriptionService.UpdateCustomerAsync(id, dto);

            if (!success)
                return NotFound(new { message = "Customer not found" });

            return Ok(new { message = "Customer updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error updating customer", error = ex.Message });
        }
    }

    [HttpDelete("customers/{id}")]
    public async Task<ActionResult> DeleteCustomer(Guid id)
    {
        try
        {
            var success = await _subscriptionService.DeleteCustomerAsync(id);

            if (!success)
                return NotFound(new { message = "Customer not found" });

            return Ok(new { message = "Customer deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting customer", error = ex.Message });
        }
    }

    [HttpPost("customers/{id}/pause")]
    public async Task<ActionResult> PauseSubscription(Guid id)
    {
        try
        {
            var success = await _subscriptionService.PauseSubscriptionAsync(id);

            if (!success)
                return NotFound(new { message = "Subscription not found" });

            return Ok(new { message = "Subscription paused successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error pausing subscription", error = ex.Message });
        }
    }

    [HttpPost("customers/{id}/resume")]
    public async Task<ActionResult> ResumeSubscription(Guid id)
    {
        try
        {
            var success = await _subscriptionService.ResumeSubscriptionAsync(id);

            if (!success)
                return NotFound(new { message = "Subscription not found" });

            return Ok(new { message = "Subscription resumed successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error resuming subscription", error = ex.Message });
        }
    }

    [HttpGet("promocodes")]
    public async Task<ActionResult<List<PromoCodeDto>>> GetActivePromoCodes()
    {
        try
        {
            var promoCodes = await _subscriptionService.GetActivePromoCodesAsync();
            return Ok(promoCodes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving promo codes", error = ex.Message });
        }
    }
    [HttpGet("customers/export")]
    public async Task<ActionResult> ExportCustomers([FromQuery] CustomerFilterDto filter)
    {
        try
        {
            var csvData = await _subscriptionService.ExportCustomersAsync(filter);

            return File(csvData, "text/csv", $"customers_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error exporting customers", error = ex.Message });
        }
    }
}

