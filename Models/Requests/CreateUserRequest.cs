namespace BankTurns.Models.Requests;

/// <summary>
/// Modelo utilizado para el registro de nuevos clientes desde el Kiosco.
/// Permite capturar la información básica necesaria para identificar al cliente en el sistema.
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Documento de identidad del cliente (Cédula, Pasaporte, etc.).
    /// Se usa como identificador clave en la búsqueda inicial del Kiosco.
    /// </summary>
    public string Document { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del cliente para ser mostrado en el ticket y la sala de espera.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
