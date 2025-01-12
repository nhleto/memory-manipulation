using fun_time.Models;

namespace fun_time.EntityHandlers;

public abstract class EntityManager
{
    protected Entity LocalPlayer { get; set; } = new();
    protected List<Entity> Entities { get; set; } = new();

    public abstract void ReadEntity(Entity entity);
    public abstract Entity GetUpdatedLocalPlayer();
    public abstract List<Entity> GetUpdatedEntities();

    public Entity GetLocalPlayer()
    {
        return LocalPlayer;
    }

    public List<Entity> GetEntities()
    {
        return Entities;
    }
}