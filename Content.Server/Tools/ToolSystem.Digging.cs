using System.Threading;
using Content.Server.Tools.Components;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Tools.Components;
using Robust.Shared.Map;

namespace Content.Server.Tools;

public sealed partial class ToolSystem
{
    private void InitializeDigging()
    {
        SubscribeLocalEvent<DiggingComponent, AfterInteractEvent>(OnDiggingAfterInteract);
        SubscribeLocalEvent<DiggingComponent, DiggingCompleteEvent>(OnDiggingComplete);
    }
    
    private void OnDiggingComplete(EntityUid uid, DiggingComponent component, DiggingCompleteEvent args)
    {
        component.CancelToken = null;
        //args.Coordinates.PryTile(EntityManager, _mapManager);
        
        if (!_mapManager.TryGetGrid(args.Coordinates.GetGridId(EntityManager), out var mapGrid))
            return;
        
        var tile = mapGrid.GetTileRef(args.Coordinates);
        var coordinates = mapGrid.GridTileToLocal(tile.GridIndices);
        var tileDef = (ContentTileDefinition)_tileDefinitionManager[tile.Tile.TypeId];
        
        // Create dirt
        if (tileDef.MaxQuantity <= 0) return;
        EntityManager.SpawnEntity("Dirt", coordinates);
        tileDef.MaxQuantity -= 1;
        if (tileDef.MaxQuantity <= 0)
        {
            EntityManager.SpawnEntity("InteractionBlocker", coordinates);
        }
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

        if (!_mapManager.TryGetGrid(clickLocation.GetGridId(EntityManager), out var mapGrid))
            return false;

        var tile = mapGrid.GetTileRef(clickLocation);
        var coordinates = mapGrid.GridTileToLocal(tile.GridIndices);

        if (!_interactionSystem.InRangeUnobstructed(user, coordinates, popup: false))
            return false;

        var tileDef = (ContentTileDefinition)_tileDefinitionManager[tile.Tile.TypeId];

        if (!tileDef.CanCrowbar)
        {
            if (tileDef.ID != "dirt_plating")
                return false;
        }
        else
        {
            return false;
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
