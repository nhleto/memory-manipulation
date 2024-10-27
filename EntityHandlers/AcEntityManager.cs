using System.Text;
using fun_time.Models;
using fun_time.Offsets;
using test_fun_proj.EntityHandlers;
using test_fun_proj.Models;

namespace fun_time.EntityHandlers;

public class AcEntityManager : EntityManager
{
    private readonly Bypass _bypass;
    private readonly nint _mainModule;

    public AcEntityManager(nint mainModule, Bypass bypass)
    {
        _mainModule = mainModule;
        _bypass = bypass;
    }

    public override void UpdateEntity(Entity entity)
    {
        entity.Health = _bypass.ReadInt(entity.BaseAddress, AcOffsets.HpOffset);
        var name = Encoding.UTF8.GetString(_bypass.ReadBytes(entity.BaseAddress, AcOffsets.NameOffset, 11));
        entity.Name = name;
    }

    public override void UpdateLocalPlayer()
    {
        LocalPlayer.BaseAddress = _bypass.ReadPointer(_mainModule, AcOffsets.LocalPlayer);
        UpdateEntity(LocalPlayer);
    }

    public override void UpdateEntities()
    {
        Entities.Clear();

        var entityListAddress = _bypass.ReadPointer(_mainModule, AcOffsets.EntityList);
        
        for (var i = 0; i < 10; i++)
        {
            var offset2 = i * AcOffsets.EntityListOffset;
            var entityAddress = _bypass.ReadPointer(entityListAddress, offset2);

            if (entityAddress == IntPtr.Zero) continue;

            var entity = new Entity
            {
                BaseAddress = entityAddress
            };
            
            UpdateEntity(entity);
            Entities.Add(entity);
        } 
    }
}