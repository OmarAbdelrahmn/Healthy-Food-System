using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Entities;
public class SupportTicket
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Open", "InProgress", "Closed"
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }


    public virtual ICollection<SupportMessage> Messages { get; set; } = [];
}
