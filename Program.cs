using System;
using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
        // Declare variables
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
        var secondPtr2Int = handleInt.AddrOfPinnedObject();
        var ptr2ptr = Marshal.AllocHGlobal(IntPtr.Size);
        Marshal.WriteIntPtr(ptr2ptr, ptr2int);  // Store the address of ptr2int in ptr2ptr
        var ptr2ptr2 = Marshal.AllocHGlobal(IntPtr.Size);
        Marshal.WriteIntPtr(ptr2ptr2, ptr2ptr); // Store the address of ptr2ptr in ptr2ptr2

        while (true)
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
        }
    }

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentProcessId();
}
