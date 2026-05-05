namespace BankTurns.Models.Requests;

/// <summary>
/// Modelo utilizado por el panel del asesor para dar por finalizada la atención de un turno.
/// </summary>
public class FinishTurnRequest
{
    /// <summary>
    /// Comentario opcional que el asesor puede agregar sobre la resolución del turno.
    /// Útil para auditoría y seguimiento del historial de atención.
    /// </summary>
    public string? Comment { get; set; }
}
