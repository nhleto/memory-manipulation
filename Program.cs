using System.Runtime.InteropServices;

namespace test_fun_proj;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class Bypass
{
    // Import necessary functions from kernel32.dll
    [DllImport("kernel32.dll")]
    static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    static extern bool CloseHandle(IntPtr hObject);

    private const int PROCESS_VM_READ = 0x0010;
    private const int PROCESS_VM_WRITE = 0x0020;
    private const int PROCESS_VM_OPERATION = 0x0008;

    private IntPtr processHandle = IntPtr.Zero;

    // Constructor
    public Bypass() { }

    // Destructor
    ~Bypass()
    {
        if (processHandle != IntPtr.Zero)
        {
            CloseHandle(processHandle);
        }
    }

    // Attach to the process by PID
    public bool Attach(int processId)
    {
        processHandle = OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, false, processId);
        return processHandle != IntPtr.Zero;
    }

    // Read from process memory
    public bool Read(IntPtr baseAddress, out int value)
    {
        var buffer = new byte[sizeof(int)];  // Buffer to hold the data read
        var bytesRead = IntPtr.Zero;

        bool success = ReadProcessMemory(processHandle, baseAddress, buffer, buffer.Length, out bytesRead);
        value = BitConverter.ToInt32(buffer, 0);  // Convert the buffer to an integer
        return success;
    }

    // Write to process memory
    public bool Write(IntPtr baseAddress, int value)
    {
        var buffer = BitConverter.GetBytes(value);  // Convert the integer to byte array
        var bytesWritten = IntPtr.Zero;

        return WriteProcessMemory(processHandle, baseAddress, buffer, buffer.Length, out bytesWritten);
    }
}

// Example usage
class Program
{
    static void Main()
    {
        Console.Write("Enter Process ID: ");
        var processId = int.Parse(Console.ReadLine());

        var bypass = new Bypass();
        if (!bypass.Attach(processId))
        {
            Console.WriteLine("Failed to attach to process. Error: " + Marshal.GetLastWin32Error());
            return;
        }

        Console.Write("Enter memory address (hex): 0x");
        var addressHex = Console.ReadLine();
        var baseAddress = new IntPtr(Convert.ToInt64(addressHex, 16));

        // Reading
        Console.WriteLine("Reading value...");
        if (bypass.Read(baseAddress, out var intRead))
        {
            Console.WriteLine($"Value read: {intRead}");
        }
        else
        {
            Console.WriteLine("Failed to read memory. Error: " + Marshal.GetLastWin32Error());
        }

        // Writing
        Console.Write("Enter new value to write: ");
        var newValue = int.Parse(Console.ReadLine());

        Console.WriteLine("Writing value...");
        if (bypass.Write(baseAddress, newValue))
        {
            Console.WriteLine("Value written successfully.");
        }
        else
        {
            Console.WriteLine("Failed to write memory. Error: " + Marshal.GetLastWin32Error());
        }
    }
}
