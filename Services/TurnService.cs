using BankTurns.Interfaces;
using BankTurns.Response;
using BankTurns.Models;
using BankTurns.Data;
using Microsoft.EntityFrameworkCore;

namespace BankTurns.Services
{
    public class TurnService : ITurnService
    {
        private readonly AppDbContext _context;
        private readonly ITurnHistoryService _historyService;

        public TurnService(AppDbContext context, ITurnHistoryService historyService)
        {
            _context = context;
            _historyService = historyService;
        }

        public async Task<ServicesResponse<Turn>> CreateAsync(int userId, BankReason reason)
        {
            var response = new ServicesResponse<Turn>();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                response.Status  = false;
                response.Message = "The User Not Found.";
                return response;
            }

            var hasActive = await _context.Turns
                .AnyAsync(t => t.UserId == userId &&
                               (t.Status == TurnStatus.Pending || t.Status == TurnStatus.InProgress));
            if (hasActive)
            {
                response.Status  = false;
                response.Message = "The user already has an active turn.";
                return response;
            }

            var today = DateTime.UtcNow.Date;
            var todayCount = await _context.Turns
                .CountAsync(t => t.CreatedAt >= today);

            var ticket = $"A{todayCount + 1:D3}";

            var turn = new Turn
            {
                UserId    = userId,
                Ticket    = ticket,
                Reason    = GetBankReasonText(reason),
                Status    = TurnStatus.Pending,
                CreatedAt = DateTime.Now
            };

            _context.Turns.Add(turn);
            await _context.SaveChangesAsync();

            await _historyService.RegisterAsync(turn.Id, null,
                TurnStatus.Pending, TurnStatus.Pending, "Turno creado");

            await _context.Entry(turn).Reference(t => t.User).LoadAsync();

            response.Status  = true;
            response.Message = $"Turn {ticket} created successfully.";
            response.Data    = turn;
            return response;
        }

        private static string GetBankReasonText(BankReason reason)
        {
            return reason switch
            {
                BankReason.BancoNacion    => "Banco Nación",
                BankReason.BancoProvincia => "Banco Provincia",
                BankReason.BancoGalicia   => "Banco Galicia",
                BankReason.BancoSantander => "Banco Santander",
                BankReason.BancoHSBC      => "Banco HSBC",
                BankReason.BancoMacro     => "Banco Macro",
                BankReason.BancoICBC      => "Banco ICBC",
                _                         => reason.ToString()
            };
        }

        public async Task<ServicesResponse<List<Turn>>> GetQueueAsync()
        {
            var response = new ServicesResponse<List<Turn>>();

            var queue = await _context.Turns
                .Include(t => t.User)
                .Where(t => t.Status == TurnStatus.Pending || t.Status == TurnStatus.InProgress)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();

            response.Status  = true;
            response.Message = $"Queue currently: {queue.Count} turn(s) pending.";
            response.Data    = queue;
            return response;
        }

        public async Task<ServicesResponse<Turn>> CallNextAsync(int advisorId)
        {
            var response = new ServicesResponse<Turn>();

            var advisor = await _context.Advisors.FindAsync(advisorId);
            if (advisor == null || advisor.Status == AdvisorStatus.Inactive)
            {
                response.Status  = false;
                response.Message = "Advisor not found or inactive.";
                return response;
            }

            var advisorBusy = await _context.Turns
                .AnyAsync(t => t.AdvisorId == advisorId && t.Status == TurnStatus.InProgress);
            if (advisorBusy)
            {
                response.Status  = false;
                response.Message = "The advisor already has a turn in progress. Finish it first.";
                return response;
            }

            var next = await _context.Turns
                .Include(t => t.User)
                .Where(t => t.Status == TurnStatus.Pending)
                .OrderBy(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (next == null)
            {
                response.Status  = false;
                response.Message = "There are no pending turns in the queue.";
                return response;
            }

            var previousStatus = next.Status;
            next.Status    = TurnStatus.InProgress;
            next.AdvisorId = advisorId;
            next.CalledAt  = DateTime.Now;

            await _context.SaveChangesAsync();

            await _historyService.RegisterAsync(next.Id, advisorId,
                previousStatus, TurnStatus.InProgress,
                $"Turn called by advisor {advisor.Name}");

            await _context.Entry(next).Reference(t => t.Advisor).LoadAsync();

            response.Status  = true;
            response.Message = $"Turn {next.Ticket} called. Client: {next.User.Name}.";
            response.Data    = next;
            return response;
        }

        public async Task<ServicesResponse<Turn>> FinishTurnAsync(int advisorId, string? comment)
        {
            var response = new ServicesResponse<Turn>();

            var turn = await _context.Turns
                .Include(t => t.User)
                .Include(t => t.Advisor)
                .FirstOrDefaultAsync(t => t.AdvisorId == advisorId &&
                                          t.Status == TurnStatus.InProgress);

            if (turn == null)
            {
                response.Status  = false;
                response.Message = "No turn in progress was found for this advisor.";
                return response;
            }

            turn.Status     = TurnStatus.Finished;
            turn.FinishedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            await _historyService.RegisterAsync(turn.Id, advisorId,
                TurnStatus.InProgress, TurnStatus.Finished,
                comment ?? "Turn Finished");

            response.Status  = true;
            response.Message = $"Turn {turn.Ticket} finished successfully.";
            response.Data    = turn;
            return response;
        }

        public async Task<ServicesResponse<Turn>> HasActiveTurnAsync(int userId)
        {
            var response = new ServicesResponse<Turn>();

            var turn = await _context.Turns
                .Include(t => t.User)
                .Include(t => t.Advisor)
                .FirstOrDefaultAsync(t => t.UserId == userId &&
                                          (t.Status == TurnStatus.Pending ||
                                           t.Status == TurnStatus.InProgress));

            if (turn == null)
            {
                response.Status  = false;
                response.Message = "The user does not have an active turn.";
                return response;
            }

            int position = 0;
            if (turn.Status == TurnStatus.Pending)
            {
                position = await _context.Turns
                    .CountAsync(t => t.Status == TurnStatus.Pending &&
                                     t.CreatedAt <= turn.CreatedAt);
            }

            response.Status  = true;
            response.Message = turn.Status == TurnStatus.Pending
                ? $"Turn {turn.Ticket} on hold. Queue position: {position}."
                : $"Turn {turn.Ticket} is being attended by {turn.Advisor?.Name ?? "advisor"}.";
            response.Data = turn;
            return response;
        }

        public async Task<ServicesResponse<List<Turn>>> GetAdvisorTurnsAsync(int advisorId)
        {
            var response = new ServicesResponse<List<Turn>>();

            var today = DateTime.Today;
            var turns = await _context.Turns
                .Include(t => t.User)
                .Where(t => t.AdvisorId == advisorId && t.CreatedAt >= today)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            response.Status  = true;
            response.Message = $"{turns.Count} turn(s) found for the advisor today.";
            response.Data    = turns;
            return response;
        }

        public async Task<ServicesResponse<Turn>> CancelTurnAsync(int userId)
        {
            var response = new ServicesResponse<Turn>();

            var turn = await _context.Turns
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId &&
                                          t.Status == TurnStatus.Pending);

            if (turn == null)
            {
                response.Status  = false;
                response.Message = "No pending turn was found to cancel.";
                return response;
            }

            turn.Status     = TurnStatus.Finished;
            turn.FinishedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            await _historyService.RegisterAsync(turn.Id, null,
                TurnStatus.Pending, TurnStatus.Finished, "Turn cancelled by user");

            response.Status  = true;
            response.Message = $"Turn {turn.Ticket} cancelled.";
            response.Data    = turn;
            return response;
        }

        public async Task<ServicesResponse<TicketDto>> GetTicketAsync(int turnId)
        {
            var response = new ServicesResponse<TicketDto>();

            var turn = await _context.Turns
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == turnId);

            if (turn == null)
            {
                response.Status  = false;
                response.Message = "Turn not found.";
                return response;
            }

            int position = await _context.Turns
                .CountAsync(t => t.Status == TurnStatus.Pending &&
                                 t.CreatedAt <= turn.CreatedAt);

            var ticket = new TicketDto
            {
                TurnId     = turn.Id,
                Ticket     = turn.Ticket,
                ClientName = turn.User.Name,
                Document   = turn.User.Document,
                Reason     = turn.Reason,
                Status     = turn.Status.ToString(),
                Position   = position,
                CreatedAt  = turn.CreatedAt,
                IssuedAt   = DateTime.Now
            };

            response.Status  = true;
            response.Message = "Ticket generated successfully.";
            response.Data    = ticket;
            return response;
        }
    }
}
