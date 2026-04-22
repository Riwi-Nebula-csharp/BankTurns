namespace BankTurns.Models;

public class TurnHistory
{
    public int Id { get; set; }
    public int TurnId { get; set; }
    public int? AdvisorId { get; set; }
    public TurnStatus PreviousStatus { get; set; }
    public TurnStatus NewStatus { get; set; }
    public string? Comment { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.Now;

    public Turn Turn { get; set; } = null!;
    public Advisor? Advisor { get; set; }
}