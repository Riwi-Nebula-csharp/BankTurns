namespace BankTurns.Models.Requests;

public class CreateTurnRequest
{
    public int UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
