using BankTurns.Models;
using BankTurns.Response;

namespace BankTurns.Interfaces
{
    public interface IUserService
    {
        Task<ServicesResponse<User?>> CreateAsync(string document, string name, string reason);
        Task<ServicesResponse<User?>> GetByDocumentAsync(string document);
    }
}