using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Configs.Traitor;
using TTT.API.Events;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.Game.Events.Body;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.CS2.Items.Station;

public static class DamageStationCollection {
  public static void AddDamageStation(this IServiceCollection collection) {
    collection.AddModBehavior<DamageStation>();
  }
}

public class DamageStation(IServiceProvider provider)
  : StationItem<TraitorRole>(provider,
    provider.GetService<IStorage<DamageStationConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new DamageStationConfig()), IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly Dictionary<string, StationInfo> killedWithStation = new();

  public override string Name => Locale[StationMsgs.SHOP_ITEM_STATION_HURT];

  public override string Description
    => Locale[StationMsgs.SHOP_ITEM_STATION_HURT_DESC];

  override protected void onInterval() {
    var players  = finder.GetOnline();
    var toRemove = new List<CPhysicsPropMultiplayer>();
    var playerMapping = players
     .Select(p => (ApiPlayer: p, GamePlayer: Converter.GetPlayer(p)))
     .Where(m
        => m.GamePlayer != null
        && !Roles.GetRoles(m.ApiPlayer).Any(r => r is TraitorRole))
     .ToList();

    // accumulate contributions per player: ApiPlayer -> list of (stationInfo, damage, gamePlayer)
    var playerDamageMap =
      new Dictionary<IOnlinePlayer, List<(StationInfo info, int damage,
        CCSPlayerController gamePlayer)>>();

    foreach (var (prop, info) in Props) {
      if (_Config.TotalHealthGiven != 0 && Math.Abs(info.HealthGiven)
        > Math.Abs(_Config.TotalHealthGiven) || !prop.IsValid
        || prop.AbsOrigin == null) {
        toRemove.Add(prop);
        continue;
      }

      var propPos = prop.AbsOrigin;

      var playerDists = playerMapping
       .Select(t => (t.ApiPlayer, Origin: t.GamePlayer!.Pawn.Value?.AbsOrigin,
          t.GamePlayer))
       .Where(t => t is { Origin: not null, ApiPlayer.IsAlive: true })
       .Select(t
          => (t.ApiPlayer, Dist: t.Origin!.Distance(propPos), t.GamePlayer))
       .Where(t => t.Dist <= _Config.MaxRange)
       .ToList();

      foreach (var (player, dist, gamePlayer) in playerDists) {
        var healthScale = 1.0 - dist / _Config.MaxRange;
        var damageAmount =
          Math.Abs((int)Math.Floor(_Config.HealthIncrements * healthScale));

        if (damageAmount <= 0) continue;

        if (!playerDamageMap.TryGetValue(player, out var list)) {
          list                    = [];
          playerDamageMap[player] = list;
        }

        list.Add((info, damageAmount, gamePlayer));
      }
    }

    // Apply accumulated damage per player once
    applyDamage(playerDamageMap);

    // remove invalid/expired props
    foreach (var prop in toRemove) Props.Remove(prop);
  }

  private void applyDamage(
    Dictionary<IOnlinePlayer, List<(StationInfo info, int damage,
      CCSPlayerController gamePlayer)>> playerDamageMap) {
    foreach (var kv in playerDamageMap) {
      var player   = kv.Key;
      var contribs = kv.Value;

      var totalDamage = contribs.Sum(c => c.damage);

      if (totalDamage <= 0) continue;

      // choose the station that contributed the most damage to attribute the kill to
      var dominantInfo = contribs.OrderByDescending(c => c.damage).First().info;
      var gamePlayer   = contribs.First().gamePlayer;

      // dispatch single PlayerDamagedEvent with total damage
      var dmgEvent = new PlayerDamagedEvent(player,
        dominantInfo.Owner as IOnlinePlayer, totalDamage) {
        Weapon = $"[{Name}]"
      };

      bus.Dispatch(dmgEvent);

      totalDamage = dmgEvent.DmgDealt;

      // if this will kill the player, attribute death to the dominant station
      if (player.Health - totalDamage <= 0) {
        killedWithStation[player.Id] = dominantInfo;
        var playerDeath = new PlayerDeathEvent(player)
         .WithKiller(dominantInfo.Owner as IOnlinePlayer)
         .WithWeapon($"[{Name}]");
        bus.Dispatch(playerDeath);
      }

      gamePlayer.EmitSound("Player.DamageFall", SELF(gamePlayer.Slot), 0.2f);

      // apply damage to player's health
      player.Health -= totalDamage;

      // update each station's HealthGiven by its own contribution
      foreach (var (info, damage, _) in contribs) info.HealthGiven += damage;
    }
  }


  private static RecipientFilter SELF(int slot) {
    return new RecipientFilter(slot);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    killedWithStation.Clear();
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRagdollSpawn(BodyCreateEvent ev) {
    if (!killedWithStation.TryGetValue(ev.Body.OfPlayer.Id,
      out var stationInfo))
      return;
    if (ev.Body.Killer != null && ev.Body.Killer.Id != ev.Body.OfPlayer.Id)
      return;
    ev.Body.Killer = stationInfo.Owner as IOnlinePlayer;
  }
}