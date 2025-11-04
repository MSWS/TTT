using CounterStrikeSharp.API;
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

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

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
     .Select(p => (ApiPlayer: p, GamePlayer: converter.GetPlayer(p)))
     .Where(m
        => m.GamePlayer != null
        && !Roles.GetRoles(m.ApiPlayer).Any(r => r is TraitorRole))
     .ToList();

    foreach (var (prop, info) in props) {
      if (_Config.TotalHealthGiven != 0 && Math.Abs(info.HealthGiven)
        > Math.Abs(_Config.TotalHealthGiven)) {
        toRemove.Add(prop);
        continue;
      }

      if (!prop.IsValid || prop.AbsOrigin == null) {
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
          (int)Math.Floor(_Config.HealthIncrements * healthScale);

        var dmgEvent = new PlayerDamagedEvent(player,
          info.Owner as IOnlinePlayer, damageAmount) { Weapon = $"[{Name}]" };

        bus.Dispatch(dmgEvent);

        damageAmount = -dmgEvent.DmgDealt;

        if (player.Health + damageAmount <= 0) {
          killedWithStation[player.Id] = info;
          var playerDeath = new PlayerDeathEvent(player)
           .WithKiller(info.Owner as IOnlinePlayer)
           .WithWeapon($"[{Name}]");
          bus.Dispatch(playerDeath);
        }

        gamePlayer.EmitSound("Player.DamageFall", SELF(gamePlayer.Slot), 0.2f);
        player.Health    += damageAmount;
        info.HealthGiven += damageAmount;
      }
    }

    foreach (var prop in toRemove) props.Remove(prop);
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