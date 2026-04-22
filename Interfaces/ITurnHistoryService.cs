using BankTurns.Models;

namespace BankTurns.Interfaces;

public interface ITurnHistoryService
{
    Task RegisterAsync(int turnId, int? advisorId, TurnStatus previous, TurnStatus next, string? comment);
    Task<List<TurnHistory>> GetByTurnAsync(int turnId);
}