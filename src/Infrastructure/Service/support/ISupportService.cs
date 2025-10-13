using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.support;
public interface ISupportService
{
    Task<int> CreateTicketAsync(string userId, CreateTicketDto dto);
    Task<List<TicketResponseDto>> GetUserTicketsAsync(string userId);
    Task<List<TicketResponseDto>> GetAllTicketsAsync(string status = null);
    Task<TicketDetailsDto> GetTicketDetailsAsync(int ticketId, string userId, bool isAdmin);
    Task<int> SendMessageAsync(int ticketId, string userId, bool isAdmin, string messageText);
    Task<bool> CloseTicketAsync(int ticketId, string userId, bool isAdmin);
    Task<bool> ReopenTicketAsync(int ticketId, string userId);
    Task<int> GetUnreadCountAsync(string userId, bool isAdmin);
}
