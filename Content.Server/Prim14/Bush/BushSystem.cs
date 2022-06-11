using System.Threading;
using Content.Server.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Prim14;
using JetBrains.Annotations;
//using Robust.Shared.Random;

namespace Content.Server.Prim14.Bush;

public sealed class BushSystem : EntitySystem
{ 
    //[Dependency] private readonly IRobustRandom _random = null!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<BushComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<BushComponent, BushReadyEvent>(OnBushReady);
        SubscribeLocalEvent<BushComponent, BushPickEvent>(BushPick);
        SubscribeLocalEvent<BushComponent, BushPickCancel>(OnBushPickCancel);
    }

    public void OnInteractHand(EntityUid uid, BushComponent component, InteractHandEvent args)
    {
        if (args.Handled || !component.Ready)
            return;
        
        TryBushPick(uid, component, args);
        
        args.Handled = true;
    }

    private void TryBushPick(EntityUid uid, BushComponent component, InteractHandEvent args)
    {
        if (component.CancelToken != null) return;
        
        component.CancelToken = new CancellationTokenSource();
        
        var doAfterEventArgs = new DoAfterEventArgs(args.User, 1, component.CancelToken.Token, uid)
        {
            BreakOnUserMove = true,
            BreakOnDamage = true,
            BreakOnStun = true,
            BreakOnTargetMove = true,
            NeedHand = true,
            TargetFinishedEvent = new BushPickEvent(args.User),
            TargetCancelledEvent = new BushPickCancel()
        };
        
        _doAfterSystem.DoAfter(doAfterEventArgs);
    }

    private void BushPick(EntityUid uid, BushComponent component, BushPickEvent args)
    {
        component.CancelToken = null;
        var playerPos = Transform(component.Owner).MapPosition;
        EntityManager.SpawnEntity(component.Loot, playerPos);
        UpdateAppearance(uid, true, false);
        component.Ready = false;
    }
    
    private void OnBushPickCancel(EntityUid uid, BushComponent component, BushPickCancel args)
    {
        component.CancelToken = null;
    }

    public override void Update(float frameTime)
    {
        foreach (var component in EntityQuery<BushComponent>())
        {
            if (component.Ready) continue;
            if (component.Accumulator < component.RespawnTime)
            {
                component.Accumulator += frameTime;
                continue;
            }
            component.Accumulator = 0;
            var ev = new BushReadyEvent(component);
            RaiseLocalEvent(component.Owner, ev, false);
        }
    }
    
    private void UpdateAppearance(EntityUid uid, bool isEmpty, bool isReady)
    {
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        appearance.SetData(BushVisuals.Empty, isEmpty);
        appearance.SetData(BushVisuals.Ready, isReady);
    }
    
    private void OnBushReady(EntityUid uid, BushComponent component, BushReadyEvent args)
    {
        component.Ready = true;
        UpdateAppearance(uid, false, true);
    }
    
    private sealed class BushReadyEvent : EntityEventArgs
    {
        private BushComponent Bush {[UsedImplicitly] get;}
        public BushReadyEvent(BushComponent bush)
        {
            Bush = bush;
        }
    }
    
    #region DoAfterClasses
    public sealed class BushPickEvent : EntityEventArgs
    {
        public readonly EntityUid User;

        public BushPickEvent(EntityUid user)
        {
            User = user;
        }
    }
    
    private sealed class BushPickCancel : EntityEventArgs { }
    
    #endregion
}