namespace BankTurns.Models.Requests;

public class LoginAdvisorRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
