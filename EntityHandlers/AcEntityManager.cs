using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using fun_time.Models;
using fun_time.Offsets;

namespace fun_time.EntityHandlers
{
    public class AcEntityManager : EntityManager
    {
        private readonly Bypass _bypass;
        private readonly nint _mainModule;

        public AcEntityManager(nint mainModule, Bypass bypass)
        {
            _mainModule = mainModule;
            _bypass = bypass;
        }

        public override void ReadEntity(Entity entity)
        {
            entity.Health = _bypass.ReadInt(entity.BaseAddress + AcOffsets.HpOffset);

            var nameBytes = _bypass.ReadBytes(entity.BaseAddress + AcOffsets.NameOffset, 11);
            entity.Name = Encoding.UTF8.GetString(nameBytes).TrimEnd('\0');  // Remove trailing null characters
        }

        public void UpdateEntityHp(Entity entity)
        {
            _bypass.WriteInt(entity.BaseAddress + AcOffsets.HpOffset, 200);
        }
        
        public override void ReadLocalPlayer()
        {
            LocalPlayer.BaseAddress = _bypass.ReadInt(_mainModule + AcOffsets.LocalPlayer);
            ReadEntity(LocalPlayer);
        }

        public override void ReadEntities()
        {
            Entities.Clear();

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
                Entities.Add(entity);
            }
        }

        public void NoRecoil()
        {
            if (LocalPlayer == null)
            {
                ReadLocalPlayer();
            }

            var player = GetLocalPlayer();
            var recoAddy = player.BaseAddress + AcOffsets.GunRecoil;
            _bypass.WriteInt(player.BaseAddress + AcOffsets.GunRecoil, 0);
        }

        public async Task KeepMeAlive()
        {
            while (true)
            {
                ReadLocalPlayer();
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

        // Calculate angle between two vectors
        private static Vector3 CalculateAngles(Vector3 source, Vector3 destination)
        {
            Vector3 delta = destination - source;
            float hypotenuse = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

            float pitch = (float)(Math.Atan2(delta.Z, hypotenuse) * (180.0 / Math.PI));
            float yaw = (float)(Math.Atan2(delta.Y, delta.X) * (180.0 / Math.PI));

            return new Vector3(pitch, yaw, 0);
        }
    }
}
