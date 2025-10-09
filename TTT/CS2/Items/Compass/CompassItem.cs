using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.CS2.Utils;
using TTT.Game.Roles;

namespace TTT.CS2.Items.Compass;

public static class CompassServiceCollection {
  public static void AddCompassServices(this IServiceCollection collection) {
    collection.AddModBehavior<CompassItem>();
  }
}

public class CompassItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider), IPluginModule {
  private readonly CompassConfig config =
    provider.GetService<IStorage<CompassConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new CompassConfig();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public override string Name => Locale[CompassMsgs.SHOP_ITEM_COMPASS];

  public override string Description
    => Locale[CompassMsgs.SHOP_ITEM_COMPASS_DESC];

  public override ShopItemConfig Config => config;

  public void Start(BasePlugin? plugin) {
    base.Start();
    plugin?.AddTimer(0.5f, tick, TimerFlags.REPEAT);
  }

  public override void OnPurchase(IOnlinePlayer player) { }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    return Shop.HasItem<CompassItem>(player) ?
      PurchaseResult.ALREADY_OWNED :
      base.CanPurchase(player);
  }

  private void tick() {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS or State.FINISHED })
      return;

    var traitors = Games.ActiveGame.Players.OfType<IOnlinePlayer>()
     .Where(p => p.IsAlive)
     .Where(p => Roles.GetRoles(p).Any(r => r is TraitorRole))
     .ToList();

    var allies = Games.ActiveGame.Players.OfType<IOnlinePlayer>()
     .Where(p => p.IsAlive)
     .Where(p => !Roles.GetRoles(p).Any(r => r is TraitorRole))
     .ToList();

    foreach (var gamePlayer in Utilities.GetPlayers()) {
      var player = converter.GetPlayer(gamePlayer);
      if (player is not IOnlinePlayer online) continue;
      if (!Shop.HasItem<CompassItem>(online)) continue;
      showRadarTo(gamePlayer, online, traitors, allies);
    }
  }

  private void showRadarTo(CCSPlayerController player, IOnlinePlayer online,
    IList<IOnlinePlayer> traitors, List<IOnlinePlayer> allies) {
    if (Games.ActiveGame?.Players == null) return;
    if (player.PlayerPawn.Value == null) return;

    var enemies = getEnemies(online, traitors, allies);
    if (enemies.Count == 0) return;
    var gameEnemies = enemies.Select(e => converter.GetPlayer(e))
     .Where(e => e != null)
     .Select(e => e!)
     .ToList();
    if (gameEnemies.Count == 0) return;

    var (nearestPlayer, distance) =
      getNearest(player, gameEnemies) ?? (null, 0);
    if (nearestPlayer == null || distance > config.MaxRange) return;
    var src = player.Pawn.Value?.AbsOrigin.Clone();
    var dst = nearestPlayer.Pawn.Value?.AbsOrigin.Clone();
    if (src == null || dst == null) return;
    var normalizedYaw = adjustGameAngle(player.PlayerPawn.Value.EyeAngles.Y);

    var diff      = (dst - src).Normalized();
    var targetYaw = MathF.Atan2(diff.Y, diff.X) * 180f / MathF.PI;
    targetYaw = adjustGameAngle(targetYaw);

    var compass = generateCompass(normalizedYaw, targetYaw);
    compass = ChatColors.Grey + compass;
    foreach (var c in "NESW".ToCharArray())
      compass = compass.Replace(c.ToString(),
        ChatColors.Default.ToString() + c + ChatColors.Grey);
    compass = compass.Replace("X", ChatColors.Red + "X" + ChatColors.Grey);

    Messenger.ScreenMsg(online,
      compass + " " + getDistanceDescription(distance));
  }

  private float adjustGameAngle(float angle) {
    return 360 - (angle + 360) % 360 + 90;
  }

  private string generateCompass(float pointing, float target) {
    return TextCompass.GenerateCompass(config.CompassFOV, config.CompassLength,
      pointing, targetDir: target);
  }

  private string getDistanceDescription(float distance) {
    return distance switch {
      > 2000 => "AWP Distance",
      > 1500 => "Scout Distance",
      > 1000 => "Rifle Distance",
      > 500  => "Pistol",
      > 250  => "Nearby",
      _      => "Knife Range"
    };
  }


  private IList<IOnlinePlayer> getEnemies(IOnlinePlayer online,
    IList<IOnlinePlayer> traitors, IList<IOnlinePlayer> allies) {
    return Roles.GetRoles(online).Any(r => r is TraitorRole) ?
      allies :
      traitors;
  }

  private (CCSPlayerController?, float)? getNearest(CCSPlayerController source,
    List<CCSPlayerController> others) {
    if (others.Count == 0) return null;
    var minDist   = float.MaxValue;
    var minPlayer = others[0];
    var src       = source.Pawn.Value?.AbsOrigin.Clone();
    if (src == null) return null;

    foreach (var player in others) {
      if (player.Pawn.Value == null) continue;

      var dist = player.Pawn.Value.AbsOrigin.Clone().DistanceSquared(src);
      if (dist >= minDist) continue;
      minDist   = dist;
      minPlayer = player;
    }

    return (minPlayer, MathF.Sqrt(minDist));
  }
}