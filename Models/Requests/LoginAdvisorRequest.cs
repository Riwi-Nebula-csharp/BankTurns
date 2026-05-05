namespace BankTurns.Models.Requests;

/// <summary>
/// Modelo utilizado para el proceso de autenticación del asesor en su panel.
/// </summary>
public class LoginAdvisorRequest
{
    /// <summary>
    /// Correo electrónico del asesor registrado en el sistema.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña asociada a la cuenta del asesor.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
