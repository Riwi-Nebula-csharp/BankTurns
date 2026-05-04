using BankTurns.Models;

namespace BankTurns.Models.Requests;

public class CreateTurnRequest
{
    public int UserId { get; set; }
    public BankReason Reason { get; set; } = BankReason.BancoNacion;
}
