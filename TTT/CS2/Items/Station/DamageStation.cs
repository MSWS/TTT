using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.CS2.Extensions;
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
     .GetResult() ?? new DamageStationConfig()) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public override string Name => Locale[StationMsgs.SHOP_ITEM_STATION_HURT];

  public override string Description
    => Locale[StationMsgs.SHOP_ITEM_STATION_HURT_DESC];

  override protected void onInterval() {
    var players = finder.GetOnline();
    foreach (var (prop, info) in props) {
      if (Math.Abs(info.HealthGiven) > Math.Abs(_Config.TotalHealthGiven)) {
        props.Remove(prop);
        continue;
      }

      var propPos = prop.AbsOrigin;
      if (propPos == null) continue;

      var playerMapping = players.Select(p
          => (ApiPlayer: p, GamePlayer: converter.GetPlayer(p)))
       .Where(m => m.GamePlayer != null);

      var playerDists = playerMapping
       .Where(t => !roles.GetRoles(t.ApiPlayer).OfType<TraitorRole>().Any())
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
        player.Health    += damageAmount;
        info.HealthGiven += damageAmount;

        gamePlayer.ExecuteClientCommand("play " + _Config.UseSound);
      }
    }
  }
}