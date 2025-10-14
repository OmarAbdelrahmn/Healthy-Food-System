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

    [HttpPost("user/{userId}/freeze")]
    public async Task<ActionResult> FreezeSubscription(string userId, [FromBody] FreezeRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "User ID is required" });

            var success = await _subscriptionService.FreezeSubscriptionAsync(
                userId,
                request?.Reason,
                request?.FreezeUntil
            );

            if (!success)
                return BadRequest(new { message = "No active subscription found for this user or subscription already frozen" });

            return Ok(new { message = $"Subscription for user {userId} frozen successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error freezing subscription", error = ex.Message });
        }
    }

    // POST: api/subscriptionfreeze/user/{userId}/unfreeze
    [HttpPost("user/{userId}/unfreeze")]
    public async Task<ActionResult> UnfreezeSubscription(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "User ID is required" });

            var success = await _subscriptionService.UnfreezeSubscriptionAsync(userId);

            if (!success)
                return BadRequest(new { message = "No frozen subscription found for this user" });

            return Ok(new { message = $"Subscription for user {userId} unfrozen successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error unfreezing subscription", error = ex.Message });
        }
    }

    // GET: api/subscriptionfreeze/user/{userId}/status
    [HttpGet("user/{userId}/status")]
    public async Task<ActionResult<UserSubscriptionStatusDto>> GetUserSubscriptionStatus(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "User ID is required" });

            var status = await _subscriptionService.GetFreezeStatusAsync(userId);

            if (status == null)
                return NotFound(new { message = "No subscriptions found for this user" });

            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving subscription status", error = ex.Message });
        }
    }

    // GET: api/subscriptionfreeze/user/{userId}/isfrozen
    [HttpGet("user/{userId}/isfrozen")]
    public async Task<ActionResult<bool>> IsUserSubscriptionFrozen(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "User ID is required" });

            var isFrozen = await _subscriptionService.IsFrozenAsync(userId);
            return Ok(new { userId, isFrozen });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error checking freeze status", error = ex.Message });
        }
    }

    // GET: api/subscriptionfreeze/frozen/count
    [HttpGet("frozen/count")]
    public async Task<ActionResult<int>> GetFrozenCount()
    {
        try
        {
            var count = await _subscriptionService.GetFrozenSubscriptionsCountAsync();
            return Ok(new { frozenSubscriptionsCount = count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving frozen count", error = ex.Message });
        }
    }
}

// Request Models
public class FreezeRequest
{
    public string Reason { get; set; }
    public DateTime? FreezeUntil { get; set; }
}


