
using BankTurns.Models; 
using BankTurns.Interfaces; 

namespace BankTurns.Services
{
    public class UserService : IUserService
    {
        Task<User?> GetByDocumentAsync(string document)
        {
            
        }

        Task<User> CreateAsync(User user)
        {
            
        }
        
        
    }
}
