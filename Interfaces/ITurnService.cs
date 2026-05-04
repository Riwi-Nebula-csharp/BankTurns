using BankTurns.Models;
using BankTurns.Response;

namespace BankTurns.Interfaces
{
    public interface ITurnService
    {
        Task<ServicesResponse<Turn>>       CreateAsync(int userId, BankReason reason);
        Task<ServicesResponse<List<Turn>>> GetQueueAsync();
        Task<ServicesResponse<Turn>>       CallNextAsync(int advisorId);
        Task<ServicesResponse<Turn>>       FinishTurnAsync(int advisorId, string? comment);
        Task<ServicesResponse<Turn>>       HasActiveTurnAsync(int userId);
        Task<ServicesResponse<List<Turn>>> GetAdvisorTurnsAsync(int advisorId);
        Task<ServicesResponse<Turn>>       CancelTurnAsync(int userId);
        Task<ServicesResponse<TicketDto>>  GetTicketAsync(int turnId);
    }
}
