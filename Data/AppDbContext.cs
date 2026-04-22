using BankTurns.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTurns.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Advisor> Advisors { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Turn> Turns { get; set; }
    public DbSet<TurnHistory> TurnHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advisor>()
            .Property(a => a.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Turn>()
            .Property(t => t.Status)
            .HasConversion<string>();

        modelBuilder.Entity<TurnHistory>()
            .Property(t => t.PreviousStatus)
            .HasConversion<string>();

        modelBuilder.Entity<TurnHistory>()
            .Property(t => t.NewStatus)
            .HasConversion<string>();
    }
}
