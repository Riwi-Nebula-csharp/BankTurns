using BankTurns.Data;
using BankTurns.Interfaces;
using BankTurns.Models;
using BankTurns.Response;
using Microsoft.EntityFrameworkCore;

namespace BankTurns.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServicesResponse<User?>> CreateAsync(string document, string name)
        {
            var response = new ServicesResponse<User?>();

            if (string.IsNullOrWhiteSpace(document) ||
                string.IsNullOrWhiteSpace(name))
            {
                response.Status  = false;
                response.Message = "Document and name are required";
                return response;
            }

            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.Document == document);

            if (existing != null)
            {
                existing.Name = name;
                await _context.SaveChangesAsync();

                response.Status  = true;
                response.Message = $"Welcome {name}.";
                response.Data    = existing;
                return response;
            }

            var user = new User
            {
                Document  = document,
                Name      = name,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            response.Status  = true;
            response.Message = $"User {name} registered successfully.";
            response.Data    = user;
            return response;
        }

        public async Task<ServicesResponse<User?>> GetByDocumentAsync(string document)
        {
            var response = new ServicesResponse<User?>();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Document == document);

            if (user == null)
            {
                response.Status  = false;
                response.Message = "User not found";
                return response;
            }

            response.Status  = true;
            response.Message = "User found.";
            response.Data    = user;
            return response;
        }
    }
}
