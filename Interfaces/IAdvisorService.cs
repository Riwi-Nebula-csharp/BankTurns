using BankTurns.Models;
using BankTurns.Response;

namespace BankTurns.Interfaces
{
    public interface IAdvisorService
    {
        Task<ServicesResponse<Advisor>>       CreateAsync(string name, string email, string password);
        Task<ServicesResponse<Advisor>>       LoginAsync(string email, string password);
        Task<ServicesResponse<List<Advisor>>> GetAllActiveAsync();
        Task<ServicesResponse<Advisor>>       ToggleStatusAsync(int advisorId);
    }
}