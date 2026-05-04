using BankTurns.Interfaces;
using BankTurns.Models;
using BankTurns.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BankTurns.Controllers;

public class KioskController : Controller
{
    private readonly IUserService _userService;
    private readonly ITurnService _turnService;
    private readonly IConfiguration _configuration;

    public KioskController(IUserService userService, ITurnService turnService, IConfiguration configuration)
    {
        _userService = userService;
        _turnService = turnService;
        _configuration = configuration;
    }

    // GET /Kiosk  — formulario paso 1 (ingresa documento)
    public IActionResult Index()
    {
        return View();
    }

    // POST /Kiosk/CheckDocument — busca usuario por documento
    [HttpPost]
    public async Task<IActionResult> CheckDocument(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
        {
            TempData["Error"] = "Por favor ingresa tu número de documento.";
            return RedirectToAction(nameof(Index));
        }

        var response = await _userService.GetByDocumentAsync(document);

        if (response.Status && response.Data != null)
        {
            // Usuario existe → verificar si ya tiene turno activo
            var activeResponse = await _turnService.HasActiveTurnAsync(response.Data.Id);
            if (activeResponse.Status && activeResponse.Data != null)
            {
                // Ya tiene turno activo → mostrar ticket directamente
                return RedirectToAction(nameof(Ticket), new { turnId = activeResponse.Data.Id });
            }

            // Usuario existe, sin turno activo → pedir motivo
            TempData["UserId"]   = response.Data.Id;
            TempData["UserName"] = response.Data.Name;
            return RedirectToAction(nameof(SelectReason));
        }

        // Usuario no existe → registrar
        TempData["Document"] = document;
        return RedirectToAction(nameof(Register));
    }

    // GET /Kiosk/Register
    public IActionResult Register()
    {
        if (TempData["Document"] == null)
            return RedirectToAction(nameof(Index));

        ViewBag.Document = TempData["Document"];
        return View();
    }

    // POST /Kiosk/Register — crea usuario y pide motivo
    [HttpPost]
    public async Task<IActionResult> Register(string document, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"]    = "El nombre es requerido.";
            TempData["Document"] = document;
            return RedirectToAction(nameof(Register));
        }

        var response = await _userService.CreateAsync(document, name);

        if (!response.Status || response.Data == null)
        {
            TempData["Error"]    = response.Message;
            TempData["Document"] = document;
            return RedirectToAction(nameof(Register));
        }

        TempData["UserId"]   = response.Data.Id;
        TempData["UserName"] = response.Data.Name;
        return RedirectToAction(nameof(SelectReason));
    }

    // GET /Kiosk/SelectReason
    public IActionResult SelectReason()
    {
        if (TempData["UserId"] == null)
            return RedirectToAction(nameof(Index));

        ViewBag.UserId   = TempData["UserId"];
        ViewBag.UserName = TempData["UserName"];
        return View();
    }

    // POST /Kiosk/CreateTurn — crea el turno con reason
    [HttpPost]
    public async Task<IActionResult> CreateTurn(CreateTurnRequest request)
    {
        if (request.UserId <= 0 || !Enum.IsDefined(typeof(BankReason), request.Reason))
        {
            TempData["Error"]    = "El motivo de visita es requerido.";
            TempData["UserId"]   = request.UserId;
            return RedirectToAction(nameof(SelectReason));
        }

        var response = await _turnService.CreateAsync(request.UserId, request.Reason);

        if (!response.Status || response.Data == null)
        {
            TempData["Error"]  = response.Message;
            TempData["UserId"] = request.UserId;
            return RedirectToAction(nameof(SelectReason));
        }

        return RedirectToAction(nameof(Ticket), new { turnId = response.Data.Id });
    }

    // GET /Kiosk/Ticket/{turnId}
    public async Task<IActionResult> Ticket(int turnId)
    {
        var response = await _turnService.GetTicketAsync(turnId);

        if (!response.Status || response.Data == null)
        {
            TempData["Error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        // Imprimir automáticamente en la impresora térmica XP-58
        try
        {
            var printerName = _configuration["Printer:Name"] ?? "XP-58";
            var ticket = response.Data;

            // Construir ticket con comandos ESC/POS
            var sb = new System.Text.StringBuilder();

            // ESC @ = Inicializar impresora
            sb.Append("\x1B\x40");

            // ESC a 1 = Centrar texto
            sb.Append("\x1B\x61\x01");

            // ESC E 1 = Negrita ON
            sb.Append("\x1B\x45\x01");
            sb.AppendLine("BANCO BANKTURNS");
            // ESC E 0 = Negrita OFF
            sb.Append("\x1B\x45\x00");

            sb.AppendLine("--------------------------------");
            sb.AppendLine($"Fecha: {ticket.IssuedAt:dd/MM/yyyy}");
            sb.AppendLine($"Hora:  {ticket.IssuedAt:hh:mm tt}");
            sb.AppendLine("--------------------------------");

            sb.AppendLine("");
            sb.AppendLine("TURNO ASIGNADO");

            // GS ! 0x11 = Doble alto y ancho (tamaño grande)
            sb.Append("\x1D\x21\x11");
            sb.AppendLine($"  {ticket.Ticket}  ");
            // GS ! 0x00 = Tamaño normal
            sb.Append("\x1D\x21\x00");

            sb.AppendLine("");
            sb.AppendLine("--------------------------------");

            // ESC E 1 = Negrita ON
            sb.Append("\x1B\x45\x01");
            sb.AppendLine(ticket.ClientName);
            // ESC E 0 = Negrita OFF
            sb.Append("\x1B\x45\x00");
            sb.AppendLine(ticket.Reason);
            sb.AppendLine("--------------------------------");
            sb.AppendLine("");
            sb.AppendLine("Espera tu llamado en la sala.");
            sb.AppendLine("Gracias por tu visita!");
            sb.AppendLine("");
            sb.AppendLine("");

            // GS V 1 = Corte parcial de papel
            sb.Append("\x1D\x56\x01");

            RawPrinterHelper.SendStringToPrinter(printerName, sb.ToString());
        }
        catch (Exception ex)
        {
            // Si hay error de impresión, no bloquear al usuario
            Console.WriteLine($"Error al imprimir ticket: {ex.Message}");
        }

        return View(response.Data);
    }
}
