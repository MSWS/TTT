using CounterStrikeSharp.API;
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
  // TODO: This seems to crash 50% of the time upon shooting.

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
  private HookResult OnTakeDamage(DynamicHook hook) {
    var playerPawn = hook.GetParam<CCSPlayerPawn>(0);
    var info       = hook.GetParam<CTakeDamageInfo>(1);

    Console.WriteLine($"OnTakeDamage called");

    var player = playerPawn.Controller.Value?.As<CCSPlayerController>();

    if (player == null || !player.IsValid || player.Pawn.Value == null
      || !player.Pawn.IsValid)
      return HookResult.Continue;

    Console.WriteLine($"Past basic checks");

    var attackerPawn = info.Attacker;
    var attacker     = attackerPawn.Value?.As<CCSPlayerController>();

    Console.WriteLine($"Attacker: {attacker?.PlayerName ?? "null"}");

    var playerGame = converter.GetPlayer(player) as IOnlinePlayer;
    var attackerGame = attacker == null ?
      null :
      converter.GetPlayer(attacker) as IOnlinePlayer;

    Console.WriteLine(
      $"PlayerGame: {playerGame?.Id ?? "null"}, AttackerGame: {attackerGame?.Id ?? "null"}");

    if (playerGame == null)
      throw new InvalidOperationException(
        "Player game object is null, this should never happen.");

    var dmgEvent = new PlayerDamagedEvent(playerGame, attackerGame,
      (int)info.Damage, player.Pawn.Value.Health - (int)info.Damage);

    bus.Dispatch(dmgEvent);

    return dmgEvent.IsCanceled ? HookResult.Handled : HookResult.Continue;
  }

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
}