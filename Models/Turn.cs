namespace BankTurns.Models;

public class Turn
{
    public int Id { get; set; }

    public string? Ticket { get; set; }
    public string? Reason { get; set; }
    public TurnStatus Status { get; set; } = TurnStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CalledAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int? AdvisorId { get; set; }
    public Advisor? Advisor { get; set; }
    public ICollection<TurnHistory> TurnHistories { get; set; } = new List<TurnHistory>();
}

public enum TurnStatus
{
    Pending,
    InProgress,
    Finished
}
