using BankTurns.Models;

namespace BankTurns.Models.Requests;

/// <summary>
/// Representa la solicitud enviada por el Kiosco para generar un nuevo turno.
/// Este modelo vincula a un cliente específico con un motivo de visita.
/// </summary>
public class CreateTurnRequest
{
    /// <summary>
    /// Identificador único del usuario (cliente) que solicita el turno.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Motivo de la visita seleccionado por el cliente (ej: Caja, Asesoría, etc.).
    /// Se basa en el enum BankReason para mantener la integridad de los datos.
    /// </summary>
    public BankReason Reason { get; set; } = BankReason.AccountManagement;
}
