using Content.Server.Materials;
using Content.Server.Popups;
using Content.Server.Prim14.Blacksmithing.Components;
using Content.Server.Prim14.TimedCooker;
using Content.Server.Stack;
using Content.Shared.Interaction;
using Content.Shared.Prim14;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Prim14.Blacksmithing;

public sealed class KilnSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly ContainerSystem _containerSystem = default!;
    private int _multiplier;

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private Queue<EntityUid> _producingAddQueue = new();

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private Queue<EntityUid> _producingRemoveQueue = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<KilnComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<KilnComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<KilnComponent, InteractHandEvent>(OnInteractHand);
    }

    private void OnComponentInit(EntityUid uid, KilnComponent component, ComponentInit args)
    {
        component.Container = _containerSystem.EnsureContainer<Container>(component.Owner, "cooker_container", out _);
        UpdateAppearance(uid, component, false);
    }

    private void OnInteractHand(EntityUid uid, KilnComponent component, InteractHandEvent args)
    {
        if (component.KilnWoodStorage <= 0)
        {
            _popupSystem.PopupEntity(Loc.GetString("timed-cooker-no-fuel"), uid, Filter.Entities(args.User));
            return;
        }
        _popupSystem.PopupEntity(
            component.IsRunning ? Loc.GetString("timed-cooker-turn-off") : Loc.GetString("timed-cooker-turn-on"), uid,
            Filter.Entities(args.User));
        component.IsRunning = !component.IsRunning;
        UpdateAppearance(uid, component, component.IsRunning);
    }

    private void OnInteractUsing(EntityUid uid, KilnComponent component, InteractUsingEvent args)
    {
        // Are we inserting wood?
        if (TryComp(args.Used, out MaterialComponent? material) && material.MaterialIds[0] == "Wood")
        {
            _multiplier = TryComp<StackComponent>(args.Used, out var stack) ? stack.Count : 4;
            component.FuelStorage += 30 * _multiplier;
            if (component.IsRunning)
                UpdateAppearance(component.Owner, component, true);
            QueueDel(args.Used);
            return;
        }

        // No? Ok can it insert, is it on the whitelist, and does it have a recipe?
        if (!component.Container.CanInsert(args.Used) ||
            component.Whitelist != null && !component.Whitelist.IsValid(args.Used) ||
            !TryComp(args.Used, out TimedCookableComponent? cookable))
        {
            _popupSystem.PopupEntity(Loc.GetString("timed-cooker-insert-fail"), uid, Filter.Entities(args.User));
            return;
        }

        // Make sure it has wood
        if (component.KilnWoodStorage <= 0)
        {
            _popupSystem.PopupEntity(Loc.GetString("timed-cooker-no-fuel"), uid, Filter.Entities(args.User));
            return;
        }

        // Make sure it's not full
        if (component.Container.ContainedEntities.Count >= component.KilnMax)
        {
            _popupSystem.PopupEntity(Loc.GetString("timed-cooker-insert-full"), uid, Filter.Entities(args.User));
            return;
        }

        if (cookable.Recipe == null ||
            !_prototypeManager.TryIndex(cookable.Recipe, out TimedCookerRecipePrototype? recipe))
            return;

        // Attempt to insert the item
        if (!component.Container.Insert(args.Used))
            return;

        // Play the inserting sound (if any)
        if (component.InsertingSound != null)
        {
            SoundSystem.Play(Filter.Pvs(component.Owner, entityManager: EntityManager), component.InsertingSound.GetSound(), component.Owner);
        }

        //Queue it up
        if (cookable.Recipe != null)
        {
            component.Queue.Enqueue(recipe);
        }
    }

    public override void Update(float frameTime)
    {
        foreach (var uid in _producingAddQueue)
        {
            EnsureComp<TimedCookerProducingComponent>(uid);
        }

        _producingAddQueue.Clear();

        foreach (var uid in _producingRemoveQueue)
        {
            RemComp<TimedCookerProducingComponent>(uid);
        }

        _producingRemoveQueue.Clear();

        foreach (var cooker in EntityQuery<KilnComponent>())
        {
            // Time frame stuff
            if (!cooker.IsRunning) continue;
            cooker.ElapsedTime += frameTime;
            if (cooker.ElapsedTime >= cooker.TimeThreshold)
            {
                // Has wood, keep cooking
                if (cooker.KilnWoodStorage > 0)
                {
                    cooker.KilnWoodStorage -= 10;
                    UpdateAppearance(cooker.Owner, cooker, cooker.KilnWoodStorage > 0);
                    cooker.ElapsedTime = 0;
                }
                // Carry on
                else
                {
                    //UpdateAppearance(cooker.Owner, false);
                    cooker.ElapsedTime = 0;
                }
            }

            if (cooker.ProducingRecipe == null)
            {
                if (cooker.Queue.Count > 0 && cooker.KilnWoodStorage > 0)
                {
                    Produce(cooker, cooker.Queue.Dequeue());
                    return;
                }
            }
            if (cooker.ProducingRecipe != null && cooker.ProducingAccumulator < cooker.ProducingRecipe.CompleteTime.TotalSeconds)
            {
                cooker.ProducingAccumulator += frameTime;
                continue;
            }

            cooker.ProducingAccumulator = 0;
            if (cooker.ProducingRecipe != null) FinishProducing(cooker.Container.ContainedEntities[0], cooker);
        }
    }

    /// <summary>
    /// If we were able to produce the recipe,
    /// spawn it and cleanup. If we weren't, just do cleanup.
    /// </summary>
    private void FinishProducing(EntityUid inside, KilnComponent component)
    {
        component.ProducingRecipe = null;
        var foundMaterial = false;

        // Check what material is inside of the jug
        if (!TryComp(inside, out BlacksmithJugComponent? jugComp) ||
            !TryComp(jugComp.MaterialSlot.Item, out MaterialComponent? materialComp))
        {
            // Check against list of
            component.Container.Remove(inside);
            return;
        }

        foreach (var mat in materialComp._materials)
        {
            foreach (var mapping in component.Results)
            {
                if (mapping.Key != mat.Key || foundMaterial) continue;
                EntityManager.SpawnEntity(mapping.Value, Comp<TransformComponent>(component.Owner).Coordinates);
                foundMaterial = true;
                break;
            }
        }

        // There is material but it's not the right material
        if (foundMaterial == false)
        {
            component.Container.Remove(inside);
            return;
        }

        // Play sound
        if (component.ProducingSound != null)
        {
            SoundSystem.Play(Filter.Pvs(component.Owner), component.ProducingSound.GetSound(), component.Owner);
        }

        // Continue to next in queue if there are items left
        if (component.Queue.Count > 0)
        {
            Produce(component, component.Queue.Dequeue());
            return;
        }
        _producingRemoveQueue.Enqueue(component.Owner);
        component.Container.CleanContainer();
    }

    /// <summary>
    /// This handles the checks to start producing an item
    /// </summary>
    private void Produce(TimedCookerComponent component, TimedCookerRecipePrototype recipe)
    {
        component.ProducingRecipe = recipe;
        _producingAddQueue.Enqueue(component.Owner);
    }

    private void UpdateAppearance(EntityUid uid, TimedCookerComponent component, bool isFired)
    {
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        appearance.SetData(KilnState.Fired, isFired);

        if (!EntityManager.TryGetComponent(uid, out PointLightComponent? light))
            return;

        component.LightOn = isFired;
        light.Enabled = component.LightOn;
    }
}
