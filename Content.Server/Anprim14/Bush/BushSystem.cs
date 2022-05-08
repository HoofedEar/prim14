using Content.Shared.Interaction;

namespace Content.Server.Anprim14.Bush;

public sealed class BushSystem : EntitySystem
{
    public bool Ready = true;
    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<BushComponent, InteractHandEvent>(OnInteractHand);
    }

    private void OnInteractHand(EntityUid uid, BushComponent component, InteractHandEvent args)
    {
        if (Ready)
        {
            // Pick the berry, set Ready to false
        }
    }
    
    // Add code in Update that checks if Ready is false. If it is, start incrementing time until threshold
    // then Ready becomes true
    
    
    // Look at visualizer stuff that changes the sprite based on Ready
}