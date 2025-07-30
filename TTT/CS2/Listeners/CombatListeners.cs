using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using TTT.API;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.Listeners;

public class CombatListeners(IEventBus bus,
  IPlayerConverter<CCSPlayerController> converter) : IPluginModule {
  public void Dispose() { }
  public string Name => "CombatListeners";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  [GameEventHandler]
  public HookResult OnPlayerDeath(EventPlayerDeath ev, GameEventInfo _) {
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;

    bus.Dispatch(new PlayerDeathEvent(converter, ev));
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnPlayerHurt(EventPlayerHurt ev, GameEventInfo _) {
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;

    bus.Dispatch(new PlayerDamagedEvent(converter, ev));
    return HookResult.Continue;
  }
}