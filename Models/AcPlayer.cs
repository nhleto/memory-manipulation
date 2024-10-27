using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace fun_time.Models;

public class AcPlayer
{
    public const nint BaseAddress = 0x18AC00;
    public const nint GameAddres = 0x400000;
    private const nint HpOffset = 0xEC;
    public static nint PositionOffset = 0x2C; 
    public static nint NameOffset = 0x205;
    public static nint HeadOffset = 0x50;

    public int Health { get; set; }
    public float Position { get; set; }
    public float HeadPosition { get; set; }
    public string? Name { get; set; }
    
    public static int ProcessId => GetAssaultCubeProcess();

    public void TopOffHealth(Bypass bypass)
    {
        /* while (true)
        {
            // Reading
            Console.WriteLine("Reading value...");
            var addy = GameAddres + BaseAddress;
            if (bypass.Read( addy + HpOffset, out var intRead))
            {
                Console.WriteLine($"Value read: {intRead}");
                if (intRead >= 200)
                {
                    continue;
                }
            }
            else
            {
                Console.WriteLine("Failed to read memory. Error: " + Marshal.GetLastWin32Error());
            }

            Console.WriteLine("Writing value...");
            if (bypass.Write(BaseAddress + HpOffset, 200))
            {
                Console.WriteLine("Value written successfully.");
            }
            else
            {
                Console.WriteLine("Failed to write memory. Error: " + Marshal.GetLastWin32Error());
            }

            Thread.Sleep(1000);
        } */
    }
    
    // Static method to find the AssaultCube process by name
    private static int GetAssaultCubeProcess()
    {
        // Get the target process by its name (ac_client)
        var processes = Process.GetProcessesByName("ac_client");
        if (processes.Length == 0)
        {
            throw new Exception("AssaultCube process not found!");
        }

        return processes[0].Id;  // Return the first found process
    }
    
    public int GetBufferSize()
    {
        var maxOffset = Math.Max(Math.Max(HpOffset, PositionOffset), 
            Math.Max(NameOffset, HeadOffset));

        return (int)maxOffset + 50; // Add extra space for the name (50 bytes)
    }
    public void ExtractFromBuffer(byte[] buffer)
    {
        Health = BitConverter.ToInt32(buffer, (int)HpOffset);
        Position = BitConverter.ToSingle(buffer, (int)PositionOffset);
        Name = Encoding.ASCII.GetString(buffer, (int)NameOffset, 50).TrimEnd('\0');
        HeadPosition = BitConverter.ToSingle(buffer, (int)HeadOffset);
    }
}