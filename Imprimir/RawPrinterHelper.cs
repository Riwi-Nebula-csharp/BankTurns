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
        int dwCount = data.Length;
        pBytes = Marshal.StringToCoTaskMemAnsi(data);

        DOCINFOA di = new DOCINFOA();
        di.pDocName = "Ticket";
        di.pDataType = "RAW";

        IntPtr hPrinter;
        OpenPrinter(printerName, out hPrinter, IntPtr.Zero);
        StartDocPrinter(hPrinter, 1, di);
        StartPagePrinter(hPrinter);

        WritePrinter(hPrinter, pBytes, dwCount, out _);

        EndPagePrinter(hPrinter);
        EndDocPrinter(hPrinter);
        ClosePrinter(hPrinter);

        Marshal.FreeCoTaskMem(pBytes);

        return true;
    }
}   