using Content.Shared.Anprim14;
using JetBrains.Annotations;
using Robust.Server.Player;
using Robust.Shared.Enums;

namespace Content.Server.Anprim14.DayNightCycle;

[UsedImplicitly]
public sealed class DayNightCycleSystem : SharedDayNightCycleSystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private float _elapsedTime;
    private int _timeThreshold = 900; // 900 = 15 mins, 600 = 10 mins, 300 = 5 mins, 60 = 1 min, 30 = 30 secs, 10 = 10 sec
    private DayNightCycleStates _current = DayNightCycleStates.Midnight;

    public override void Initialize()
    {
        base.Initialize();
        _playerManager.PlayerStatusChanged += PlayerStatusChanged;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _elapsedTime += frameTime;

        if (!(_elapsedTime >= _timeThreshold)) return;
        _current = _current switch
        {
            DayNightCycleStates.Midnight => DayNightCycleStates.Dawn,
            DayNightCycleStates.Dawn => DayNightCycleStates.Morning,
            DayNightCycleStates.Morning => DayNightCycleStates.Noon,
            DayNightCycleStates.Noon => DayNightCycleStates.Afternoon,
            DayNightCycleStates.Afternoon => DayNightCycleStates.Dusk,
            DayNightCycleStates.Dusk => DayNightCycleStates.Midnight,
            _ => throw new ArgumentOutOfRangeException()
        };

        Logger.Info("DayNight Cycle is now: " + _current);

        RaiseNetworkEvent(new DayNightCycleMessage(_current));
        _elapsedTime = 0f;
    }

    private void PlayerStatusChanged(object? blah, SessionStatusEventArgs args)
    {
        if (args.NewStatus == SessionStatus.Connected)
        {
            RaiseNetworkEvent(new DayNightCycleMessage(_current));
        }
    }
}
