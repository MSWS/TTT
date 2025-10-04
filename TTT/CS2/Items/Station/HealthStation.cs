using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Configs.Detective;
using TTT.API.Extensions;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.Game.Roles;

namespace TTT.CS2.Items.Station;

public static class HealthStationCollection {
  public static void AddHealthStation(this IServiceCollection collection) {
    collection.AddModBehavior<HealthStation>();
  }
}

public class HealthStation(IServiceProvider provider)
  : StationItem<DetectiveRole>(provider,
    provider.GetService<IStorage<HealthStationConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new HealthStationConfig()) {
  public override string Name => Locale[StationMsgs.SHOP_ITEM_STATION_HEALTH];

  public override string Description
    => Locale[StationMsgs.SHOP_ITEM_STATION_HEALTH_DESC];

  override protected void onInterval() {
    var players  = Utilities.GetPlayers();
    var toRemove = new List<CPhysicsPropMultiplayer>();
    foreach (var (prop, info) in props) {
      if (_Config.TotalHealthGiven != 0
        && Math.Abs(info.HealthGiven) > _Config.TotalHealthGiven) {
        toRemove.Add(prop);
        continue;
      }

      var propPos = prop.AbsOrigin;
      if (propPos == null) continue;

      var playerDists = players
       .Select(p => (Player: p, Pos: p.Pawn.Value?.AbsOrigin))
       .Where(t => t is { Pos: not null, Player.Pawn.Value.Health: > 0 })
       .Select(t => (t.Player, Dist: t.Pos!.Distance(propPos)))
       .Where(t => t.Dist <= _Config.MaxRange)
       .ToList();

      foreach (var (player, dist) in playerDists) {
        var maxHp       = player.Pawn.Value?.MaxHealth ?? 100;
        var healthScale = 1.0 - dist / _Config.MaxRange;
        var healAmount =
          (int)Math.Ceiling(_Config.HealthIncrements * healthScale);
        var newHealth = Math.Min(player.GetHealth() + healAmount, maxHp);
        player.SetHealth(newHealth);
        info.HealthGiven += healAmount;

        player.ExecuteClientCommand("play " + _Config.UseSound);
      }
    }

    foreach (var prop in toRemove) props.Remove(prop);
  }
}