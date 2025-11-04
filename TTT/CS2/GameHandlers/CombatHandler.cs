using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.API;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.CS2.GameHandlers;

public class CombatHandler(IServiceProvider provider) : IPluginModule {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IAliveSpoofer spoofer =
    provider.GetRequiredService<IAliveSpoofer>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public void Start() { }

  public void Dispose() { }

  /// <summary>
  /// </summary>
  /// <param name="ev"></param>
  /// <param name="info"></param>
  /// <returns></returns>
  [UsedImplicitly]
  [GameEventHandler(HookMode.Pre)]
  public HookResult OnPlayerDeath_Pre(EventPlayerDeath ev, GameEventInfo info) {
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;
    var deathEvent = new PlayerDeathEvent(converter, ev);

    hideAndTrackStats(ev, player);

    if (games.ActiveGame is not { State: State.IN_PROGRESS })
      return HookResult.Continue;

    if (ev.Attacker != null) {
      ev.FireEventToClient(ev.Attacker);
      var apiPlayer = converter.GetPlayer(ev.Attacker);
      var role      = roles.GetRoles(apiPlayer);
      if (role.Any(r => r is TraitorRole))
        foreach (var p in Utilities.GetPlayers()) {
          var apiP = converter.GetPlayer(p);
          if (apiP.Id == apiPlayer.Id) continue;
          var r = roles.GetRoles(converter.GetPlayer(p));
          if (role.Intersect(r).Any()) ev.FireEventToClient(p);
        }
    }

    info.DontBroadcast = true;
    spoofer.SpoofAlive(player);
    Server.NextWorldUpdateAsync(() => bus.Dispatch(deathEvent));
    return HookResult.Continue;
  }

  [UsedImplicitly]
  [GameEventHandler(HookMode.Pre)]
  public HookResult OnPlayerDamage(EventPlayerHurt ev, GameEventInfo info) {
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;

    hideAndTrackStats(ev);

    return HookResult.Continue;
  }

  private void hideAndTrackStats(EventPlayerDeath ev,
    CCSPlayerController player) {
    var victimStats = player.ActionTrackingServices?.MatchStats;
    if (victimStats != null) {
      victimStats.Deaths -= 1;
      Utilities.SetStateChanged(player, "CCSPlayerController",
        "m_pActionTrackingServices");
    }

    var killerStats = ev.Attacker?.ActionTrackingServices?.MatchStats;

    if (ev.Attacker != null) {
      Utilities.SetStateChanged(ev.Attacker,
        "CCSPlayerController_ActionTrackingServices",
        "m_pActionTrackingServices");
      if (killerStats == null) return;
      killerStats.Kills         -= 1;
      killerStats.Damage        -= ev.DmgHealth;
      killerStats.UtilityDamage =  0;
      if (ev.Attacker.ActionTrackingServices != null)
        ev.Attacker.ActionTrackingServices.NumRoundKills--;
      Utilities.SetStateChanged(ev.Attacker, "CSPerRoundStats_t", "m_iDamage");
      Utilities.SetStateChanged(ev.Attacker, "CSPerRoundStats_t",
        "m_iUtilityDamage");
      Utilities.SetStateChanged(ev.Attacker, "CCSPlayerController",
        "m_pActionTrackingServices");
    }

    var assisterStats = ev.Assister?.ActionTrackingServices?.MatchStats;
    if (assisterStats != null && assisterStats != killerStats)
      assisterStats.Assists -= 1;

    if (ev.Assister != null)
      Utilities.SetStateChanged(ev.Assister, "CCSPlayerController",
        "m_pActionTrackingServices");
  }

  private void hideAndTrackStats(EventPlayerHurt ev) {
    var attackerStats = ev.Attacker?.ActionTrackingServices?.MatchStats;

    if (attackerStats == null) return;
    if (ev.Attacker == null) return;
    attackerStats.Damage -= ev.DmgHealth;
    Utilities.SetStateChanged(ev.Attacker, "CCSPlayerController",
      "m_pActionTrackingServices");
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnPlayerHurt(EventPlayerHurt ev, GameEventInfo _) {
    // DamageCanceler already handles this on non-Windows platforms
    if (!OperatingSystem.IsWindows()) return HookResult.Continue;
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;

    var dmgEvent = new PlayerDamagedEvent(converter, ev);

    bus.Dispatch(dmgEvent);

    ev.Health = dmgEvent.HpLeft;
    ev.Armor  = dmgEvent.ArmorRemaining;

    return dmgEvent.IsCanceled ? HookResult.Handled : HookResult.Continue;
  }
}