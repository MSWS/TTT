using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using TTT.API;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.GameHandlers;

public class CombatHandler(IEventBus bus,
  IPlayerConverter<CCSPlayerController> converter) : IPluginModule {
  public string Name => "CombatListeners";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  /// <summary>
  /// </summary>
  /// <param name="ev"></param>
  /// <param name="_"></param>
  /// <returns></returns>
  [GameEventHandler]
  public HookResult OnPlayerDeath(EventPlayerDeath ev, GameEventInfo _) {
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;
    var deathEvent = new PlayerDeathEvent(converter, ev);

    Server.NextWorldUpdateAsync(() => bus.Dispatch(deathEvent));
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnPlayerHurt(EventPlayerHurt ev, GameEventInfo _) {
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;

    var dmgEvent = new PlayerDamagedEvent(converter, ev);

    bus.Dispatch(dmgEvent);

    var pawn = player.Pawn.Value;
    if (pawn != null && pawn.IsValid) {
      pawn.Health = dmgEvent.HpLeft;
      if (player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid)
        player.PlayerPawn.Value.ArmorValue = dmgEvent.ArmorRemaining;
    }

    if (dmgEvent.IsCanceled) return HookResult.Handled;

    return HookResult.Continue;
  }

  public void Dispose() { }
}