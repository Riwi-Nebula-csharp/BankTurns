using System;
using System.Runtime.InteropServices;

public class RawPrinterHelper
{
    [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true)]
    public static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pd);

    [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true)]
    public static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true)]
    public static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In] DOCINFOA di);

    [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true)]
    public static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true)]
    public static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true)]
    public static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true)]
    public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    [StructLayout(LayoutKind.Sequential)]
    public class DOCINFOA
    {
        public string pDocName;
        public string pOutputFile;
        public string pDataType;
    }

    public static bool SendStringToPrinter(string printerName, string data)
    {
        IntPtr pBytes;
        int dwCount;
        
        // Convert the string to bytes (using ANSI/ASCII for ESC/POS)
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(data);
        dwCount = bytes.Length;
        
        // Allocate unmanaged memory for the bytes
        pBytes = Marshal.AllocCoTaskMem(dwCount);
        Marshal.Copy(bytes, 0, pBytes, dwCount);

        DOCINFOA di = new DOCINFOA();
        di.pDocName = "BankTurns Ticket";
        di.pDataType = "RAW";

        IntPtr hPrinter = IntPtr.Zero;
        bool success = false;

        if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
        {
            // Intento de fallback: Buscar la impresora por defecto del sistema
            Console.WriteLine($"Impresora '{printerName}' no encontrada. Intentando con la impresora por defecto...");
            
            try {
                // System.Drawing.Printing.PrinterSettings proporciona la impresora por defecto
                string defaultPrinter = new System.Drawing.Printing.PrinterSettings().PrinterName;
                if (!string.IsNullOrEmpty(defaultPrinter) && OpenPrinter(defaultPrinter, out hPrinter, IntPtr.Zero)) {
                    Console.WriteLine($"Usando impresora por defecto: {defaultPrinter}");
                } else {
                    hPrinter = IntPtr.Zero;
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error al intentar obtener la impresora por defecto: {ex.Message}");
                hPrinter = IntPtr.Zero;
            }
        }

        if (hPrinter != IntPtr.Zero)
        {
            if (StartDocPrinter(hPrinter, 1, di))
            {
                if (StartPagePrinter(hPrinter))
                {
                    int dwWritten = 0;
                    success = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                    EndPagePrinter(hPrinter);
                }
                EndDocPrinter(hPrinter);
            }
            ClosePrinter(hPrinter);
        }
        else
        {
            Console.WriteLine($"No se pudo abrir ninguna impresora para el ticket.");
        }

        Marshal.FreeCoTaskMem(pBytes);
        return success;
    }
}   