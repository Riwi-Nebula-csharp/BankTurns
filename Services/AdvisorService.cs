using BankTurns.Data;
using BankTurns.Interfaces;
using BankTurns.Models;
using BankTurns.Response;
using Microsoft.EntityFrameworkCore;

namespace BankTurns.Services
{
    public class AdvisorService : IAdvisorService
    {
        private readonly AppDbContext _context;

        public AdvisorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServicesResponse<Advisor>> CreateAsync(string name, string email, string password)
        {
            var response = new ServicesResponse<Advisor>();

            if (await _context.Advisors.AnyAsync(a => a.Email == email))
            {
                response.Status  = false;
                response.Message = "An advisor with this email already exists.";
                return response;
            }

            var advisor = new Advisor
            {
                Name         = name,
                Email        = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Status       = AdvisorStatus.Active,
                CreatedAt    = DateTime.Now
            };

            _context.Advisors.Add(advisor);
            await _context.SaveChangesAsync();

            response.Status  = true;
            response.Message = $"Advisor {name} created successfully.";
            response.Data    = advisor;
            return response;
        }

        public async Task<ServicesResponse<Advisor>> LoginAsync(string email, string password)
        {
            var response = new ServicesResponse<Advisor>();

            var advisor = await _context.Advisors
                .FirstOrDefaultAsync(a => a.Email == email && a.Status == AdvisorStatus.Active);

            if (advisor == null || !BCrypt.Net.BCrypt.Verify(password, advisor.PasswordHash))
            {
                response.Status  = false;
                response.Message = "Invalid credentials.";
                return response;
            }

            response.Status  = true;
            response.Message = $"Welcome, {advisor.Name}.";
            response.Data    = advisor;
            return response;
        }

        public async Task<ServicesResponse<List<Advisor>>> GetAllActiveAsync()
        {
            var response = new ServicesResponse<List<Advisor>>();

            var advisors = await _context.Advisors
                .Where(a => a.Status == AdvisorStatus.Active)
                .ToListAsync();

            response.Status  = true;
            response.Message = $"{advisors.Count} active advisor(s) found.";
            response.Data    = advisors;
            return response;
        }

        public async Task<ServicesResponse<Advisor>> ToggleStatusAsync(int advisorId)
        {
            var response = new ServicesResponse<Advisor>();

            var advisor = await _context.Advisors.FindAsync(advisorId);
            if (advisor == null)
            {
                response.Status  = false;
                response.Message = "Advisor not found.";
                return response;
            }

            advisor.Status = advisor.Status == AdvisorStatus.Active
                ? AdvisorStatus.Inactive
                : AdvisorStatus.Active;

            await _context.SaveChangesAsync();

            response.Status  = true;
            response.Message = $"Advisor status updated to {advisor.Status}.";
            response.Data    = advisor;
            return response;
        }
    }
}
