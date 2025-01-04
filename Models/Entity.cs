namespace fun_time.Models;

public class Entity
{
    public nint BaseAddress { get; set; }
    public int Health { get; set; }
    public (float X, float Y, float Z) Position { get; set; }
    public (float X, float Y, float Z) HeadPosition { get; set; }
    public string? Name { get; set; }
    public string? Team { get; set; }
    public bool IsDead { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
}
