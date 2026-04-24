using BankTurns.Data;
using BankTurns.Response;
using BankTurns.Interfaces;
using BankTurns.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTurns.Services 
{
    public class  UserService : IUserService
    {
        private AppDbContext _dbContext;
        private IUserService _userServiceImplementation;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

         public async Task<ServicesResponse<User>>   CreateAsync(string document, string name)
        {
            var User  = await _dbContext.Users.FirstOrDefaultAsync(u => u.Document == document);

            if (User == null)
            {
                User = new User()
                {
                    Name = name,
                    Document = document,
                    Reason = string.Empty
                };
                _dbContext.AddAsync(User);
               await _dbContext.SaveChangesAsync();
               return new ServicesResponse<User>()
               {
                   Status = true,
                   Message = $"User {name} was created successfully.",
                   Data = User
               };
            }

            return new ServicesResponse<User>()
            {
                Status = false,
                Message = $"The user {name} is already registered ",
                Data = User
            };


        }

     
    }
}
