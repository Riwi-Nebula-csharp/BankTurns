using BankTurns.Data;
using BankTurns.Interfaces;
using BankTurns.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTurns.Services
{
    public class TurnHistoryService : ITurnHistoryService
    {
        private readonly AppDbContext _context;

        public TurnHistoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegisterAsync(int turnId, int? advisorId,
            TurnStatus previous, TurnStatus next, string? comment)
        {
            var history = new TurnHistory
            {
                TurnId         = turnId,
                AdvisorId      = advisorId,
                PreviousStatus = previous,
                NewStatus      = next,
                Comment        = comment,
                ChangedAt      = DateTime.Now
            };

            _context.TurnHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TurnHistory>> GetByTurnAsync(int turnId)
        {
            return await _context.TurnHistories
                .Include(h => h.Advisor)
                .Where(h => h.TurnId == turnId)
                .OrderBy(h => h.ChangedAt)
                .ToListAsync();
        }
        // jaja
    }
}