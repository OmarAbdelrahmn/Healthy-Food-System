using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Entities;
public class SupportMessage
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string SenderId { get; set; }
    public bool IsAdmin { get; set; }
    public string MessageText { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }

    public virtual SupportTicket Ticket { get; set; } = default!;
}
