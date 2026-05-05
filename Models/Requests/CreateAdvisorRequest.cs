namespace BankTurns.Models.Requests;

/// <summary>
/// Clase DTO (Data Transfer Object) para la creación de nuevos asesores.
/// Se utiliza para capturar los datos desde el formulario de registro de asesores
/// y transportarlos hacia el controlador y el servicio de negocio.
/// </summary>
public class CreateAdvisorRequest
{
    /// <summary>
    /// Nombre completo del asesor.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico institucional que servirá como nombre de usuario para el login.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña de acceso (será encriptada antes de guardarse en la base de datos).
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
