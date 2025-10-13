using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.support;
internal class data
{
}
public class CreateTicketDto
{
    public string Subject { get; set; }
    public string InitialMessage { get; set; }
}

public class SendMessageDto
{
    public int TicketId { get; set; }
    public string MessageText { get; set; }
}

public class TicketResponseDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Subject { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UnreadMessagesCount { get; set; }
    public string LastMessage { get; set; }
}

public class MessageResponseDto
{
    public int Id { get; set; }
    public string SenderId { get; set; }
    public bool IsAdmin { get; set; }
    public string MessageText { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}

public class TicketDetailsDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Subject { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MessageResponseDto> Messages { get; set; }
}