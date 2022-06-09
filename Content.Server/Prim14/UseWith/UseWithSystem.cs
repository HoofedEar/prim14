using Content.Server.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Random.Helpers;
using Content.Shared.Storage;
using Robust.Shared.Random;

namespace Content.Server.Prim14.UseWith;

public sealed class UseWithSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = null!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<UseWithComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<UseWithComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<UseWithComponent, UseWithEvent>(TryUseWith);
    }

    private void OnInteractUsing(EntityUid uid, UseWithComponent component, InteractUsingEvent args)
    {
        if (component.UseWithWhitelist?.IsValid(args.Used) == false) return;

        for (var i = 0; i < component.SpawnCount; i++)
        {
            var getResult = EntitySpawnCollection.GetSpawns(component.Results, _random)[0];
            var playerPos = Transform(uid).MapPosition;
            var spawnPos = playerPos.Offset(_random.NextVector2(0.3f));
            var spawnResult = Spawn(getResult, spawnPos);
            spawnResult.RandomOffset(0.25f);
        }

        QueueDel(uid);
    }

    private void OnUseInHand(EntityUid uid, UseWithComponent component, UseInHandEvent args)
    {
        if (args.Handled || !component.UseInHand)
            return;

        args.Handled = OnUseWith(uid, component, args);
    }

    private bool OnUseWith(EntityUid uid, UseWithComponent component, UseInHandEvent args)
    {
        _doAfterSystem.DoAfter(new DoAfterEventArgs(args.User, 1, component.CancelToken!.Token)
            {
                BreakOnUserMove = true,
                BreakOnDamage = true,
                BreakOnStun = true,
                BreakOnTargetMove = true,
                MovementThreshold = 0.01f,
                TargetFinishedEvent = new UseWithEvent(uid, component),
                NeedHand = true,
            });

        return true;
    }

    private void TryUseWith(EntityUid uid, UseWithComponent component, UseWithEvent args)
    {
        for (var i = 0; i < args.UseWith.SpawnCount; i++)
        {
            var getResult = EntitySpawnCollection.GetSpawns(args.UseWith.Results, _random)[0];
            var playerPos = Transform(uid).MapPosition;
            var spawnPos = playerPos.Offset(_random.NextVector2(0.3f));
            var spawnResult = Spawn(getResult, spawnPos);
            spawnResult.RandomOffset(0.25f);
        }

        QueueDel(uid);
    }
}
