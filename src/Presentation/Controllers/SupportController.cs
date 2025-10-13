using Infrastructure.Service.support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class SupportController(ISupportService supportService) : ControllerBase
    {
        private readonly ISupportService _supportService = supportService;

        [HttpPost("tickets")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            var userId = User.GetUserId();

            var ticketId = await _supportService.CreateTicketAsync(userId!, dto);

            return Ok(new { ticketId, message = "Ticket created successfully" });
        }

        [HttpGet("tickets")]
        public async Task<IActionResult> GetUserTickets()
        {
            var userId = User.GetUserId();

            var tickets = await _supportService.GetUserTicketsAsync(userId!);

            return Ok(tickets);
        }

        // Admin gets all tickets
        [HttpGet("admin/tickets")]
        public async Task<IActionResult> GetAllTickets([FromQuery] string status = null)
        {
            var tickets = await _supportService.GetAllTicketsAsync(status);

            return Ok(tickets);
        }

        // Get ticket details with messages
        [HttpGet("tickets/{ticketId}")]
        public async Task<IActionResult> GetTicketDetails(int ticketId)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var ticket = await _supportService.GetTicketDetailsAsync(ticketId, userId!, isAdmin);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        // Send message
        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var messageId = await _supportService.SendMessageAsync(dto.TicketId, userId!, isAdmin, dto.MessageText);

            if (messageId == 0)
                return NotFound();

            return Ok(new { messageId });
        }

        // Close ticket (Admin or User)
        [HttpPut("tickets/{ticketId}/close")]
        public async Task<IActionResult> CloseTicket(int ticketId)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var success = await _supportService.CloseTicketAsync(ticketId, userId!, isAdmin);

            if (!success)
                return NotFound();

            return Ok(new { message = "Ticket closed successfully" });
        }

        // Reopen ticket
        [HttpPut("tickets/{ticketId}/reopen")]
        public async Task<IActionResult> ReopenTicket(int ticketId)
        {
            var userId = User.GetUserId();

            var success = await _supportService.ReopenTicketAsync(ticketId, userId!);

            if (!success)
                return NotFound();

            return Ok(new { message = "Ticket reopened successfully" });
        }

        // Get unread message count
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var count = await _supportService.GetUnreadCountAsync(userId!, isAdmin);

            return Ok(new { unreadCount = count });
        }
    }
}