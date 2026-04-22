using BankTurns.Models;

namespace BankTurns.Interfaces;

public interface IUserService
{
    Task<User?> GetByDocumentAsync(string document);
    Task<User> CreateAsync(User user);
}
