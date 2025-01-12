namespace fun_time.Offsets;

public static class AcOffsets
{
    // Base pointers
    public static int LocalPlayer = 0x17E0A8;
    public static int EntityList = 0x18AC04;
    public static int ViewMatrix = 0x17DFD0;  // Updated to match C++ source
    public static int GameMode = 0x18ABF8;    // Added from C++ source
    
    // Entity list offset (not in C++ source but keeping if you need it)
    public static int EntityListOffset = 0x4;
    
    // Player offsets (from Player struct)
    public static int HeadOffset = 0x4;       // hPos in C++ struct
    public static int Angles = 0x28;      // pPos in C++ struct
    public static int AnglesOffset = 0x34;    // Where pYaw starts
    public static int PitchOffset = 0x38;     // Added from C++ struct
    public static int HpOffset = 0xEC;        // pHealth in C++ struct
    public static int NameOffset = 0x205;     // pName in C++ struct
    public static int TeamOffset = 0x30C;     // pTeam in C++ struct
    public static int DeadOffset = 0x318;     // Updated from C++ struct (was 0xB4)
    
    // Weapon related offsets (added from C++ source)
    public static int GunInfoClass = 0xC;
    public static int CurrentWeaponClass = 0x14;
    public static int GunClipAmmo = 0x0;
    public static int GunRecoil = 0x60;
    public static int GunFireRate = 0x48;
    public static int GunKnockBack = 0x54;
    public static int GunAnimation = 0x5A;
}