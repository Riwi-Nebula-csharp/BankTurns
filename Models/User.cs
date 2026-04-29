namespace BankTurns.Models;

public class User
{
    public int Id { get; set; }
    public string? Document { get; set; } 
    public string? Name { get; set; }
    public string? Reason { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Turn> Turns { get; set; } = new List<Turn>();
}
    