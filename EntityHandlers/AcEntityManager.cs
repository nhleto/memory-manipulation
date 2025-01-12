using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using fun_time.Models;
using fun_time.Offsets;

namespace fun_time.EntityHandlers;

public class AcEntityManager : EntityManager
{
    private readonly Bypass _bypass;
    private readonly nint _mainModule;
    private const int RightMouseKey = 0x02;

    public AcEntityManager(nint mainModule, Bypass bypass)
    {
        _mainModule = mainModule;
        _bypass = bypass;
    }

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);


    public override void ReadEntity(Entity entity)
    {
        entity.Health = _bypass.ReadInt(entity.BaseAddress + AcOffsets.HpOffset);
        entity.HeadPosition = _bypass.ReadVec(entity.BaseAddress + AcOffsets.HeadOffset);
        entity.ViewMatrix = _bypass.ReadVec(entity.BaseAddress + AcOffsets.AnglesOffset);
        entity.Team = _bypass.ReadInt(entity.BaseAddress + AcOffsets.TeamOffset);
        entity.IsDead = _bypass.ReadInt(entity.BaseAddress + AcOffsets.DeadOffset);

        var nameBytes = _bypass.ReadBytes(entity.BaseAddress + AcOffsets.NameOffset, 11);
        entity.Name = Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'); // Remove trailing null characters
    }

    private void UpdateEntityHp(Entity entity)
    {
        _bypass.WriteInt(entity.BaseAddress + AcOffsets.HpOffset, 200);
    }

    public override Entity GetUpdatedLocalPlayer()
    {
        LocalPlayer.BaseAddress = _bypass.ReadInt(_mainModule + AcOffsets.LocalPlayer);
        ReadEntity(LocalPlayer);
        return LocalPlayer;
    }

    public override List<Entity> GetUpdatedEntities()
    {
        Entities.Clear();
        var localPlayer = GetUpdatedLocalPlayer();

        var entityListAddress = (nint)_bypass.ReadInt(_mainModule + AcOffsets.EntityList);

        for (var i = 0; i < 10; i++)
        {
            var entityOffset = i * AcOffsets.EntityListOffset;
            var entityAddress = (nint)_bypass.ReadInt(entityListAddress + entityOffset);

            if (entityAddress == IntPtr.Zero) continue;

            var entity = new Entity
            {
                BaseAddress = entityAddress
            };

            ReadEntity(entity);
            entity.Magnitude = CalculateMagnitude(localPlayer, entity);
            Entities.Add(entity);
        }

        return Entities;
    }

    public void LoadAimBot()
    {
        var thread = new Thread(GetClosestEnemyHead);
        thread.Start();
    }

    private void GetClosestEnemyHead()
    {
        while (true)
        {
            var localPlayer = GetUpdatedLocalPlayer();
            var entities = GetUpdatedEntities().OrderBy(ent => ent.Magnitude)
                .Where(ent => ent.Team != localPlayer.Team && ent.IsDead != 1);

            foreach (var entity in entities)
            {
                if (GetAsyncKeyState(RightMouseKey) < 0)
                {
                    var angles = CalculateAngles(localPlayer, entity);
                    Aim(localPlayer, angles.X, angles.Y);
                    
                    break;
                }
            }

            Thread.Sleep(20);
        }
    }

    public async Task KeepMeAlive()
    {
        while (true)
        {
            GetUpdatedLocalPlayer();
            var player = GetLocalPlayer();

            Console.WriteLine(player.Name);
            Console.WriteLine(player.Health);
            if (player.Health < 200)
            {
                player.Health = 200;
                Console.WriteLine("health is below 100");
                UpdateEntityHp(player);
            }


            await Task.Delay(100);
        }
    }

    private float CalculateMagnitude(Entity localPlayer, Entity destinationEntity)
    {
        return (float)Math.Sqrt(Math.Pow(destinationEntity.HeadPosition.X - localPlayer.HeadPosition.X, 2) +
                                Math.Pow(destinationEntity.HeadPosition.Y - localPlayer.HeadPosition.Y, 2) +
                                Math.Pow(destinationEntity.HeadPosition.Z - localPlayer.HeadPosition.Z, 2));
    }


    // Calculate angle between two vectors
    private static Vector2 CalculateAngles(Entity localPlayer, Entity destinationEntity)
    {
        var deltaX = destinationEntity.HeadPosition.X - localPlayer.HeadPosition.X;
        var deltaY = destinationEntity.HeadPosition.Y - localPlayer.HeadPosition.Y;

        var x = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI) + 90;
        var deltaZ = destinationEntity.HeadPosition.Z - localPlayer.HeadPosition.Z;
        var distance = CalculateDistance(localPlayer, destinationEntity);

        var y = (float)(Math.Atan2(deltaZ, distance) * 180 / Math.PI);
        return new Vector2(x, y);
    }

    private void Aim(Entity ent, float x, float y)
    {
        _bypass.WriteFloat(ent.BaseAddress, AcOffsets.AnglesOffset, x);
        _bypass.WriteFloat(ent.BaseAddress, AcOffsets.AnglesOffset + 0x4, y);
    }

    private static float CalculateDistance(Entity localPlayer, Entity destEnt)
    {
        return (float)Math.Sqrt(Math.Pow(destEnt.HeadPosition.X - localPlayer.HeadPosition.X, 2) +
                                Math.Pow(destEnt.HeadPosition.Y - localPlayer.HeadPosition.Y, 2));
    }
}