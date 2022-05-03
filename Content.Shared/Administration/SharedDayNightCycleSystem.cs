using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable, NetSerializable]
public abstract class SharedDayNightCycleSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<DayNightCycleMessage>(OnDayNightCycleMessage);
    }
    
    protected virtual void OnDayNightCycleMessage(DayNightCycleMessage message, EntitySessionEventArgs eventArgs)
    {
        // Specific side code in target.
    }

    [Serializable, NetSerializable]
    public sealed class DayNightCycleMessage : EntityEventArgs
    {
        public DayNightCycleStates State { get; }
        
        public DayNightCycleMessage(DayNightCycleStates state)
        {
            State = state;
        }
    }
    
    public enum DayNightCycleStates
    {
        Midnight,
        Dawn,
        Morning,
        Noon,
        Afternoon,
        Dusk
    }
}