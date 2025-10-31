using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using JetBrains.Annotations;
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

  private readonly IDictionary<int, RoundData> roundStats =
    new Dictionary<int, RoundData>();

  public void Dispose() { }

  [UsedImplicitly]
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

  // Needs to be higher so we detect the kill before the game ends
  // in the case that this is the last player
  [UsedImplicitly]
  [EventHandler(Priority = Priority.HIGH)]
  public void OnKill(PlayerDeathEvent ev) {
    var killer = ev.Killer == null ? null : converter.GetPlayer(ev.Killer);
    var assister =
      ev.Assister == null ? null : converter.GetPlayer(ev.Assister);

    if (killer != null) {
      roundStats.TryGetValue(killer.Slot, out var def);
      def ??= new RoundData();
      def.Kills++;
      roundStats[killer.Slot] = def;
    }

    if (assister != null && assister != killer) {
      roundStats.TryGetValue(assister.Slot, out var def);
      def ??= new RoundData();
      def.Assists++;
      roundStats[assister.Slot] = def;
    }
  }

  [UsedImplicitly]
  [EventHandler(Priority = Priority.HIGH)]
  public void OnDamage(PlayerDamagedEvent ev) {
    var attacker =
      ev.Attacker == null ? null : converter.GetPlayer(ev.Attacker);
    if (attacker == null) return;

    roundStats.TryGetValue(attacker.Slot, out var def);
    def                       ??= new RoundData();
    def.Damage                +=  ev.DmgDealt;
    roundStats[attacker.Slot] =   def;
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState == State.IN_PROGRESS) {
      revealedDeaths.Clear();
      roundStats.Clear();
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
      player.PawnIsAlive = false;
      Utilities.SetStateChanged(player, "CCSPlayerController",
        "m_bPawnIsAlive");
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
     .Where(p => p.IsValid && roundStats.ContainsKey(p.Slot));

    foreach (var player in online) {
      var stats = player.ActionTrackingServices?.MatchStats;
      if (stats == null) continue;

      if (!roundStats.TryGetValue(player.Slot, out var data)) continue;

      stats.Kills   += data.Kills;
      stats.Assists += data.Assists;
      Utilities.SetStateChanged(player, "CCSPlayerController",
        "m_pActionTrackingServices");
    }
  }

  private record RoundData {
    public int Assists;
    public int Damage;
    public int Kills;
  }
}