using System.Numerics;

namespace fun_time.Models;

public class Entity
{
    public nint BaseAddress { get; set; }
    public int Health { get; set; }
    public Vector3 HeadPosition { get; set; }
    public Vector3 ViewMatrix { get; set; }
    public string? Name { get; set; }
    public int Team { get; set; }
    public int IsDead { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
    public float Magnitude { get; set; }
}
