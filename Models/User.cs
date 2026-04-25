namespace BankTurns.Models;

public class User
{
    public int Id { get; set; }
    public string Document { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Turn> Turns { get; set; } = new List<Turn>();
}
