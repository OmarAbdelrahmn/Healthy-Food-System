using Domain.Models.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.support;
public class SupportService(ApplicationDbContext dbContext) : ISupportService
{
    private readonly ApplicationDbContext _context = dbContext;



    public async Task<int> CreateTicketAsync(string userId, CreateTicketDto dto)
    {
        var ticket = new SupportTicket
        {
            UserId = userId,
            Subject = dto.Subject,
            Status = "Open",
            CreatedAt = DateTime.UtcNow
        };

        _context.SupportTickets.Add(ticket);
        await _context.SaveChangesAsync();

        var message = new SupportMessage
        {
            TicketId = ticket.Id,
            SenderId = userId,
            IsAdmin = false,
            MessageText = dto.InitialMessage,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.SupportMessages.Add(message);
        await _context.SaveChangesAsync();

        return ticket.Id;
    }

    public async Task<List<TicketResponseDto>> GetUserTicketsAsync(string userId)
    {
        var tickets = await _context.SupportTickets
            .Where(t => t.UserId == userId)
            .Select(t => new TicketResponseDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Subject = t.Subject,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UnreadMessagesCount = t.Messages.Count(m => !m.IsRead && m.IsAdmin),
                LastMessage = t.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => m.MessageText)
                    .FirstOrDefault()!
            })
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tickets;
    }

    public async Task<List<TicketResponseDto>> GetAllTicketsAsync(string status = null)
    {
        var query = _context.SupportTickets.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(t => t.Status == status);
        }

        var tickets = await query
            .Select(t => new TicketResponseDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Subject = t.Subject,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UnreadMessagesCount = t.Messages.Count(m => !m.IsRead && !m.IsAdmin),
                LastMessage = t.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => m.MessageText)
                    .FirstOrDefault()!
            })
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tickets;
    }

    public async Task<TicketDetailsDto> GetTicketDetailsAsync(int ticketId, string userId, bool isAdmin)
    {
        var ticket = await _context.SupportTickets
            .Where(t => t.Id == ticketId && (isAdmin || t.UserId == userId))
            .FirstOrDefaultAsync();

        if (ticket == null)
            return null;

        var messages = await _context.SupportMessages
            .Where(m => m.TicketId == ticketId)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageResponseDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                IsAdmin = m.IsAdmin,
                MessageText = m.MessageText,
                SentAt = m.SentAt,
                IsRead = m.IsRead
            })
            .ToListAsync();

        var unreadMessages = await _context.SupportMessages
            .Where(m => m.TicketId == ticketId && !m.IsRead && m.IsAdmin != isAdmin)
            .ToListAsync();

        if (unreadMessages.Any())
        {
            unreadMessages.ForEach(m => m.IsRead = true);
            await _context.SaveChangesAsync();
        }

        return new TicketDetailsDto
        {
            Id = ticket.Id,
            UserId = ticket.UserId,
            Subject = ticket.Subject,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            Messages = messages
        };
    }

    public async Task<int> SendMessageAsync(int ticketId, string userId, bool isAdmin, string messageText)
    {
        var ticket = await _context.SupportTickets.FindAsync(ticketId);
        if (ticket == null)
            return 0;

        if (!isAdmin && ticket.UserId != userId)
            return 0;

        var message = new SupportMessage
        {
            TicketId = ticketId,
            SenderId = userId,
            IsAdmin = isAdmin,
            MessageText = messageText,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.SupportMessages.Add(message);

        if (ticket.Status == "Closed")
        {
            ticket.Status = "Open";
        }
        else if (isAdmin && ticket.Status == "Open")
        {
            ticket.Status = "InProgress";
        }

        await _context.SaveChangesAsync();

        return message.Id;
    }

    public async Task<bool> CloseTicketAsync(int ticketId, string userId, bool isAdmin)
    {
        var ticket = await _context.SupportTickets.FindAsync(ticketId);
        if (ticket == null)
            return false;

        if (!isAdmin && ticket.UserId != userId)
            return false;

        ticket.Status = "Closed";
        ticket.ClosedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ReopenTicketAsync(int ticketId, string userId)
    {
        var ticket = await _context.SupportTickets
            .Where(t => t.Id == ticketId && t.UserId == userId)
            .FirstOrDefaultAsync();

        if (ticket == null)
            return false;

        ticket.Status = "Open";
        ticket.ClosedAt = null;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetUnreadCountAsync(string userId, bool isAdmin)
    {
        if (isAdmin)
        {
            return await _context.SupportMessages
                .Where(m => !m.IsRead && !m.IsAdmin)
                .CountAsync();
        }
        else
        {
            return await _context.SupportMessages
                .Where(m => !m.IsRead && m.IsAdmin && m.Ticket.UserId == userId)
                .CountAsync();
        }
    }
}
