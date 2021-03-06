using System.Linq;
using System.Threading;
using Content.Server.Popups;
using Content.Server.Prim14.Blocker;
using Content.Server.Tools.Components;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Tools.Components;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Server.Tools;

public sealed partial class ToolSystem
{
    [Dependency] private readonly PopupSystem _popup = default!;
    public bool IsPrying;
    private void InitializeDigging()
    {
        SubscribeLocalEvent<DiggingComponent, AfterInteractEvent>(OnDiggingAfterInteract);
        SubscribeLocalEvent<DiggingComponent, DiggingCompleteEvent>(OnDiggingComplete);
    }

    private void OnDiggingComplete(EntityUid uid, DiggingComponent component, DiggingCompleteEvent args)
    {
        component.CancelToken = null;

        if (!_mapManager.TryGetGrid(args.Coordinates.GetGridUid(EntityManager), out var mapGrid))
            return;

        var tile = mapGrid.GetTileRef(args.Coordinates);
        var coordinates = mapGrid.GridTileToLocal(tile.GridIndices);
        var tileDef = (ContentTileDefinition)_tileDefinitionManager[tile.Tile.TypeId];

        // Create dirt or clay depending on what is defined by the tile
        if (tileDef.Quantity <= 0) return;
        EntityManager.SpawnEntity(tileDef.Loot, coordinates);
        tileDef.Quantity -= 1;

        // Once we have dug all the dirt up, place a marker to block it
        if (tileDef.Quantity > 0) return;
        EntityManager.SpawnEntity("InteractionBlocker", coordinates);
        tileDef.Quantity = 3;
    }

    private void OnDiggingAfterInteract(EntityUid uid, DiggingComponent component, AfterInteractEvent args)
    {
        if (args.Handled || !args.CanReach) return;

        if (TryDigging(args.User, component, args.ClickLocation))
            args.Handled = true;
    }

    private bool TryDigging(EntityUid user, DiggingComponent component, EntityCoordinates clickLocation)
    {
        if (component.CancelToken != null)
        {
            component.CancelToken.Cancel();
            component.CancelToken = null;
            return false;
        }

        if (!TryComp<ToolComponent?>(component.Owner, out var tool) && component.ToolComponentNeeded)
            return false;

        if (!_mapManager.TryGetGrid(clickLocation.GetGridUid(EntityManager), out var mapGrid))
            return false;

        var tile = mapGrid.GetTileRef(clickLocation);
        var coordinates = mapGrid.GridTileToLocal(tile.GridIndices);

        if (!_interactionSystem.InRangeUnobstructed(user, coordinates, popup: false))
            return false;

        //var gridUid = EntityManager.GetComponent<TransformComponent>(component.Owner).GridUid;
        EntityLookupSystem lookup = default!;

        foreach (var entity in lookup.GetEntitiesIntersecting(tile).ToArray())
        {
            if (!EntityManager.HasComponent<BlockerComponent>(entity))
            {
                continue;
            }

            // TODO Localise
            _popup.PopupEntity("The ground is too hard.", user, Filter.Entities(user));
            return false;
        }

        var tileDef = (ContentTileDefinition)_tileDefinitionManager[tile.Tile.TypeId];

        if (!tileDef.CanCrowbar)
        {
            if (tileDef.ID == "dirt_plating" && tileDef.Quantity > 0)
            {
                IsPrying = false;
            }
            else
            {
                return false;
            }
        }
        else
        {
            IsPrying = true;
        }

        var token = new CancellationTokenSource();
        component.CancelToken = token;

        //var gridId = EntityManager.GetComponent<TransformComponent>(component.Owner).GridID;
        //var lookup = Get<EntityLookupSystem>();

        UseTool(
            component.Owner,
            user,
            null,
            0f,
            component.Delay,
            new [] {component.QualityNeeded},
            new DiggingCompleteEvent
            {
                Coordinates = clickLocation,
            },
            toolComponent: tool,
            doAfterEventTarget: component.Owner,
            cancelToken: token.Token);

        return true;
    }

    private sealed class DiggingCompleteEvent : EntityEventArgs
    {
        public EntityCoordinates Coordinates { get; init; }
    }
}
