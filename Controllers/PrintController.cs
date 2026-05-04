using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Printing;

namespace BankTurns.Controllers;

public class PrintController : Controller
{ 
    
    
    public IActionResult Test()
    {
        string printerName = "XP-58";

        string ticket = "HOLA MUNDO\n\n\n";

        RawPrinterHelper.SendStringToPrinter(printerName, ticket);

        return Content("Enviado");
    }
    
    
    //
    // public IActionResult ImprimirTicket()
    // {
    //     string printerName = "XP-58"; // CAMBIA por el nombre exacto
    //
    //     string ticket = "";
    //     ticket += "MI NEGOCIO\n";
    //     ticket += "----------------------\n";
    //     ticket += "Producto A   $5.000\n";
    //     ticket += "Producto B   $10.000\n";
    //     ticket += "----------------------\n";
    //     ticket += "TOTAL: $15.000\n\n\n";
    //
    //     RawPrinterHelper.SendStringToPrinter(printerName, ticket);
    //
    //     return Content("Ticket impreso");
    // }
}