using Content.Server.Anprim14.Blacksmithing.Components;
using Content.Server.Anprim14.TimedCooker;
using Content.Server.Materials;
using Content.Server.Popups;
using Content.Shared.Interaction;
using Robust.Server.Containers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Anprim14.Blacksmithing;

public sealed class KilnSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly ContainerSystem _containerSystem = default!;

    private readonly Dictionary<string, string> _kilnMapping = new()
    {
        {"Tin", "JugMoltenTin"},
        {"Copper", "JugMoltenCopper"},
        {"Bronze", "JugMoltenBronze"},
        {"Iron", "JugMoltenIron"},
        {"Gold", "JugMoltenGold"}
    };

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private Queue<EntityUid> _producingAddQueue = new();
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private Queue<EntityUid> _producingRemoveQueue = new();
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<KilnComponent, ComponentInit>(HandleTimedCookerInit);
        SubscribeLocalEvent<KilnComponent, InteractUsingEvent>(OnInteractUsing);
    }

    private void HandleTimedCookerInit(EntityUid uid, KilnComponent component, ComponentInit args)
    {
        component.Container = _containerSystem.EnsureContainer<Container>(component.Owner, "cooker_container", out _);
    }

    private void OnInteractUsing(EntityUid uid, KilnComponent component, InteractUsingEvent args)
    {
        // Check if the item can be insert, and that it's on the whitelist
        if (!component.Container.CanInsert(args.Used) &&
            component.Whitelist?.IsValid(args.Used) == false)
        {
            _popupSystem.PopupEntity(Loc.GetString("timed-cooker-insert-fail"), uid, Filter.Entities(args.User));
            return;
        }
        
        // Make sure it has a valid recipe
        if (!TryComp(args.Used, out TimedCookableComponent? cookable))
        {
            // Unless it's made of wood
            if (TryComp(args.Used, out MaterialComponent? material) && material.MaterialIds[0] != "Wood") return;
            component.KilnWoodStorage += 10;
            QueueDel(args.Used);
            return;
        }

        // Make sure it's not full
        if (component.Queue.Count >= component.KilnMax)
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
            cooker.ElapsedTime += frameTime;
            if (cooker.ElapsedTime >= cooker.TimeThreshold)
            {
                // Has wood, keep cooking
                if (cooker.KilnWoodStorage > 0)
                {
                    cooker.KilnWoodStorage -= 1;
                }
                // No more wood, stop cooking
                else
                {
                    return;
                }
            }
            
            if (cooker.ProducingRecipe == null)
            {
                if (cooker.Queue.Count > 0)
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
            foreach (var mapping in _kilnMapping)
            {
                if (mapping.Key == mat.Key && foundMaterial == false)
                {
                    EntityManager.SpawnEntity(mapping.Value, Comp<TransformComponent>(component.Owner).Coordinates);
                    foundMaterial = true;
                    break;
                }
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
    private void Produce(KilnComponent component, TimedCookerRecipePrototype recipe)
    {
        component.ProducingRecipe = recipe;
        _producingAddQueue.Enqueue(component.Owner);
    }
}
