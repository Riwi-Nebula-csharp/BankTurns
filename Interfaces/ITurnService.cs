using BankTurns.Models;

namespace BankTurns.Interfaces;

public interface ITurnService
{
    Task<Turn> CreateAsync(int userId);
    
    Task<List<Turn>> GetQueueAsync();
    Task<Turn?> GetCurrentTurnAsync(int advisorId);
    Task<Turn?> CallNextAsync(int advisorId);
    Task<Turn?> FinishTurnAsync(int advisorId, string? comment);
     Task<bool> HasActiveTurnAsync(int userId);
}   