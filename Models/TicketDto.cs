namespace BankTurns.Models
{
    public class TicketDto
    {
        public int    TurnId     { get; set; }
        public string? Ticket     { get; set; } 
        public string? ClientName { get; set; }
        public string? Document   { get; set; } 
        public string? Reason     { get; set; }
        public string? Status     { get; set; } 
        public int    Position   { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime IssuedAt  { get; set; }
    }
}