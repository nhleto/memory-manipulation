using test_fun_proj.Models;

namespace test_fun_proj.EntityHandlers;

public abstract class EntityManager
{
    protected Entity LocalPlayer { get; set; } = new();
    protected List<Entity> Entities { get; set; } = new();

    public abstract void UpdateEntity(Entity entity);
    public abstract void UpdateLocalPlayer();
    public abstract void UpdateEntities();

    public Entity GetLocalPlayer()
    {
        return LocalPlayer;
    }

    public List<Entity> GetEntities()
    {
        return Entities;
    }
}