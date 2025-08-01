using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using TTT.API;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.GameHandlers;

public class CombatHandler(IEventBus bus,
  IPlayerConverter<CCSPlayerController> converter) : IPluginModule {
  public void Dispose() {
    VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(OnTakeDamage,
      HookMode.Pre);
  }

  public string Name => "CombatListeners";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() {
    VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnTakeDamage,
      HookMode.Pre);
  }

  /// <summary>
  ///   We need to use this to be able to cancel damage
  /// </summary>
  /// <param name="hook"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  private HookResult OnTakeDamage(DynamicHook hook) {
    var playerPawn = hook.GetParam<CCSPlayerPawn>(0);
    var info       = hook.GetParam<CTakeDamageInfo>(1);

    var player = playerPawn.Controller.Value?.As<CCSPlayerController>();

    if (player == null || !player.IsValid || player.Pawn.Value == null)
      return HookResult.Continue;

    var attackerPawn = info.Attacker;
    var attacker     = attackerPawn.Value?.As<CCSPlayerController>();

    var playerGame = converter.GetPlayer(player) as IOnlinePlayer;
    var attackerGame = attacker == null ?
      null :
      converter.GetPlayer(attacker) as IOnlinePlayer;

    if (playerGame == null)
      throw new InvalidOperationException(
        "Player game object is null, this should never happen.");

    var dmgEvent = new PlayerDamagedEvent(playerGame, attackerGame,
      (int)info.Damage, player.Pawn.Value.Health - (int)info.Damage);

    bus.Dispatch(dmgEvent);
    return dmgEvent.IsCanceled ? HookResult.Handled : HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnPlayerDeath(EventPlayerDeath ev, GameEventInfo _) {
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;

    bus.Dispatch(new PlayerDeathEvent(converter, ev));
    return HookResult.Continue;
  }

  // [GameEventHandler]
  // public HookResult OnPlayerHurt(EventPlayerHurt ev, GameEventInfo _) {
  //   var player = ev.Userid;
  //   if (player == null) return HookResult.Continue;
  //
  //   var dmgEvent = new PlayerDamagedEvent(converter, ev);
  //
  //   bus.Dispatch(dmgEvent);
  //
  //   if (dmgEvent.IsCanceled) { return HookResult.Handled; }
  //
  //   return HookResult.Continue;
  // }
}