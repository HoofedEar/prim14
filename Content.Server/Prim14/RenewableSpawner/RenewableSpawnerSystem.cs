using Robust.Shared.Random;

namespace Content.Server.Prim14.RenewableSpawner;

public sealed class RenewableSpawnerSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _robustRandom = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        foreach (var spawner in EntityQuery<RenewableSpawnerComponent>())
        {
            spawner.ElapsedTime += frameTime;

            if (!(spawner.ElapsedTime >= spawner.IntervalSeconds)) return;
            Respawn(spawner.Owner, spawner);
            spawner.ElapsedTime = 0;
        }
    }

    /// <summary>
    /// Spawn the chosen entity, then delete the spawner.
    /// </summary>
    /// <param name="uid">Owner of the component</param>
    /// <param name="component">Component passthrough</param>
    public void Respawn(EntityUid uid, RenewableSpawnerComponent component)
    {
        if (!TryComp<TransformComponent>(component.Owner, out var transformComp))
            return;

        var entity = _robustRandom.Pick(component.Prototypes);
        EntityManager.SpawnEntity(entity, transformComp.Coordinates);
        EntityManager.DeleteEntity(uid);
    }
}
