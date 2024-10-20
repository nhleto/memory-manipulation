using System;
using System.Runtime.InteropServices;

class Program
{
    private static uint PROCESS_READ_ACCESS = 0x0010;
    
    static void Main()
    {
        // Declare variables
        var intRead = 0;
        var readString = "";
        var varInt = 123456;
        var varString = "DefaultString";
        const int arraySize = 128;
        var arrChar = new char[arraySize];
        var arrCharContent = "Long char array right there ->";
        Array.Copy(arrCharContent.ToCharArray(), arrChar, arrCharContent.Length);

        // Create "pointer-like" behavior using GCHandle
        var handleInt = GCHandle.Alloc(varInt, GCHandleType.Pinned);
        var handleString = GCHandle.Alloc(varString, GCHandleType.Pinned);
        var handleArrChar = GCHandle.Alloc(arrChar, GCHandleType.Pinned);

        // Use IntPtr to simulate pointer behavior
        var ptr2int = handleInt.AddrOfPinnedObject();
        var ptr2ptr = Marshal.AllocHGlobal(IntPtr.Size);
        Marshal.WriteIntPtr(ptr2ptr, ptr2int);  // Store the address of ptr2int in ptr2ptr
        var ptr2ptr2 = Marshal.AllocHGlobal(IntPtr.Size);
        Marshal.WriteIntPtr(ptr2ptr2, ptr2ptr); // Store the address of ptr2ptr in ptr2ptr2
        
        var ptrToString = handleString.AddrOfPinnedObject();
        var stringSize = (varString.Length + 1) * sizeof(char); // +1 for the null terminator
        var bufferString = new byte[stringSize];

        // Open the current process
        var processId = GetCurrentProcessId();
        var processHandle = OpenProcess(PROCESS_READ_ACCESS, false, processId);

        if (processHandle == IntPtr.Zero)
        {
            Console.WriteLine("Failed to open process.");
            return;
        }

        var success = ReadProcessMemory(processHandle, ptrToString, bufferString, (uint)bufferString.Length, out var bytesReadString);
        if (success && bytesReadString == stringSize)
        {
            // Convert byte array to integer
            readString = System.Text.Encoding.Unicode.GetString(bufferString, 0, bytesReadString);
            Console.WriteLine($"Read string: {readString}");
        }
        else
        {
            Console.WriteLine($"Failed to read memory. {GetLastError()}");
        }

        // Cleanup handles
        handleInt.Free();
        handleString.Free();
        handleArrChar.Free();
        CloseHandle(processHandle);

        /* while (true)
        {
            // Print process ID in decimal
            Console.WriteLine($"Process ID (in decimal): {GetCurrentProcessId()}");

            // Print variable addresses and values, with memory addresses in hexadecimal (uppercase) with "0x" prefix
            Console.WriteLine($"varInt (0x{ptr2int.ToString("X")}) = {varInt}");
            Console.WriteLine($"second pointer varInt (0x{secondPtr2Int.ToString("X")}) = {varInt}");
            Console.WriteLine($"varString (0x{handleString.AddrOfPinnedObject().ToString("X")}) = {varString}");
            Console.WriteLine($"arrChar (0x{handleArrChar.AddrOfPinnedObject().ToString("X")}) = {new string(arrChar)}");

            // Print "pointer" information (using IntPtr) with uppercase hexadecimal addresses
            Console.WriteLine($"ptr2int (0x{ptr2ptr.ToString("X")}) = 0x{ptr2int.ToString("X")}");
            Console.WriteLine($"ptr2ptr (0x{ptr2ptr2.ToString("X")}) = 0x{ptr2ptr.ToString("X")}");
            Console.WriteLine($"ptr2ptr2 (0x{ptr2ptr2.ToString("X")}) = 0x{ptr2ptr2.ToString("X")}");

            // Prompt user to press enter
            Console.WriteLine("Press ENTER to print again.");
            Console.ReadLine();

            // Print separator line
            Console.WriteLine(new string('-', 50));
        } */


    }

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentProcessId();
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);
    
    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);
    
    [DllImport("kernel32.dll")]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, out int lpNumberOfBytesRead);
    
    [DllImport("kernel32.dll")]
    private static extern uint GetLastError();
}