namespace BankTurns.Models.Requests;

public class CreateUserRequest
{
    public string Document { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
