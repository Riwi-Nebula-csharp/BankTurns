namespace BankTurns.Models;

public class Advisor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public AdvisorStatus Status { get; set; } = AdvisorStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Turn> Turns { get; set; } = new List<Turn>();
    public ICollection<TurnHistory> TurnHistories { get; set; } = new List<TurnHistory>();
}

public enum AdvisorStatus
{
    Active,
    Inactive
}