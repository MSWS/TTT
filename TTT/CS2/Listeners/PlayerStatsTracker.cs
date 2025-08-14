using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Body;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;

namespace TTT.CS2.Listeners;

public class PlayerStatsTracker(IServiceProvider provider) : IListener {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly ISet<int> revealedDeaths = new HashSet<int>();

  private readonly IDictionary<int, (int, int)> roundKillsAndAssists =
    new Dictionary<int, (int, int)>();

  public void Dispose() { }

  [EventHandler(Priority = Priority.MONITOR)]
  public void OnIdentify(BodyIdentifyEvent ev) {
    var gamePlayer = converter.GetPlayer(ev.Body.OfPlayer);
    if (gamePlayer == null || !gamePlayer.IsValid) return;

    var stats = gamePlayer.ActionTrackingServices?.MatchStats;
    if (stats == null) return;

    stats.Deaths++;
    Utilities.SetStateChanged(gamePlayer, "CCSPlayerController",
      "m_pActionTrackingServices");
    revealedDeaths.Add(gamePlayer.Slot);
  }

  [EventHandler]
  public void OnKill(PlayerDeathEvent ev) {
    var killer = ev.Killer == null ? null : converter.GetPlayer(ev.Killer);
    var assister =
      ev.Assister == null ? null : converter.GetPlayer(ev.Assister);

    if (killer != null) {
      roundKillsAndAssists.TryGetValue(killer.Slot, out var def);
      def.Item1++;
      roundKillsAndAssists[killer.Slot] = def;
    }

    if (assister != null && assister != killer) {
      roundKillsAndAssists.TryGetValue(assister.Slot, out var def);
      def.Item2++;
      roundKillsAndAssists[assister.Slot] = def;
    }
  }

  [EventHandler]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState == State.IN_PROGRESS) {
      revealedDeaths.Clear();
      roundKillsAndAssists.Clear();
      return;
    }

    if (ev.NewState != State.FINISHED) return;
    revealDeaths();
    revealKills();
  }

  private void revealDeaths() {
    var online = finder.GetOnline()
     .Where(p => !p.IsAlive)
     .Select(p => converter.GetPlayer(p))
     .OfType<CCSPlayerController>()
     .Where(p => p.IsValid && !revealedDeaths.Contains(p.Slot));

    foreach (var player in online) {
      var stats = player.ActionTrackingServices?.MatchStats;
      if (stats == null) continue;

      stats.Deaths++;
      Utilities.SetStateChanged(player, "CCSPlayerController",
        "m_pActionTrackingServices");
    }
  }

  private void revealKills() {
    var online = finder.GetOnline()
     .Select(p => converter.GetPlayer(p))
     .OfType<CCSPlayerController>()
     .Where(p => p.IsValid && roundKillsAndAssists.ContainsKey(p.Slot));

    foreach (var player in online) {
      var stats = player.ActionTrackingServices?.MatchStats;
      if (stats == null) continue;

      var (kills, assists) =  roundKillsAndAssists[player.Slot];
      stats.Kills          += kills;
      stats.Assists        += assists;
      Utilities.SetStateChanged(player, "CCSPlayerController",
        "m_pActionTrackingServices");
    }
  }
}