using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Events;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.CS2.Utils;
using TTT.Game.Events.Game;
using TTT.Game.Roles;

namespace TTT.CS2.Items.Compass;

/// <summary>
/// Base compass that renders a heading toward the nearest target returned by GetTargets.
/// Child classes decide which targets to expose and who owns the item.
/// </summary>
public abstract class AbstractCompassItem<TRole> : RoleRestrictedItem<TRole>,
  IListener, IPluginModule where TRole : class, IRole {
  protected readonly CompassConfig config;
  protected readonly IPlayerConverter<CCSPlayerController> Converter;
  protected readonly ISet<IPlayer> Owners = new HashSet<IPlayer>();

  protected AbstractCompassItem(IServiceProvider provider) : base(provider) {
    config = provider.GetService<IStorage<CompassConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new CompassConfig();

    Converter =
      provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();
  }

  public override ShopItemConfig Config => config;

  public void Start(BasePlugin? plugin) {
    base.Start();
    plugin?.AddTimer(0.1f, Tick, TimerFlags.REPEAT);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState == State.FINISHED) Owners.Clear();
  }

  /// <summary>
  /// Return world positions to point at for this player.
  /// </summary>
  protected abstract IList<Vector> GetTargets(IOnlinePlayer requester);

  /// <summary>
  /// Whether this player currently owns/has this compass effect.
  /// </summary>
  protected abstract bool OwnsItem(IOnlinePlayer player);

  public override void OnPurchase(IOnlinePlayer player) { Owners.Add(player); }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    return OwnsItem(player) ?
      PurchaseResult.ALREADY_OWNED :
      base.CanPurchase(player);
  }

  private void Tick() {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS or State.FINISHED })
      return;

    foreach (var player in Owners.OfType<IOnlinePlayer>()) {
      var gamePlayer = Converter.GetPlayer(player);
      if (gamePlayer == null) continue;
      ShowCompass(gamePlayer, player);
    }
  }

  private void ShowCompass(CCSPlayerController viewer, IOnlinePlayer online) {
    if (Games.ActiveGame?.Players == null) return;
    if (viewer.PlayerPawn.Value == null) return;

    var src = viewer.Pawn.Value?.AbsOrigin.Clone();
    if (src == null) return;

    var targets = GetTargets(online).ToList();
    if (targets.Count == 0) return;

    var (nearest, distance) = GetNearestVector(src, targets);
    if (nearest == null || distance > config.MaxRange) return;

    var normalizedYaw = AdjustGameAngle(viewer.PlayerPawn.Value.EyeAngles.Y);

    var diff      = (nearest - src).Normalized();
    var targetYaw = MathF.Atan2(diff.Y, diff.X) * 180f / MathF.PI;
    targetYaw = AdjustGameAngle(targetYaw);

    var compass = GenerateCompass(normalizedYaw, targetYaw);
    compass = "<font color=\"#777777\">" + compass;
    foreach (var c in "NESW".ToCharArray())
      compass = compass.Replace(c.ToString(),
        $"</font><font color=\"#FFFF00\">{c}</font><font color=\"#777777\">");
    compass = compass.Replace("X",
      "</font><font color=\"#FF0000\">X</font><font color=\"#777777\">");
    compass += "</font>";

    viewer.PrintToCenterHtml($"{compass} {GetDistanceDescription(distance)}");
  }

  private static float AdjustGameAngle(float angle) {
    return 360 - (angle + 360) % 360 + 90;
  }

  private string GenerateCompass(float pointing, float target) {
    return TextCompass.GenerateCompass(config.CompassFOV, config.CompassLength,
      pointing, targetDir: target);
  }

  private static string GetDistanceDescription(float distance) {
    return distance switch {
      > 2000 => "AWP Distance",
      > 1500 => "Scout Distance",
      > 1000 => "Rifle Distance",
      > 500  => "Pistol",
      > 250  => "Nearby",
      _      => "Knife Range"
    };
  }

  private static (Vector?, float) GetNearestVector(in Vector src,
    IList<Vector> targets) {
    var     minDistSq = float.MaxValue;
    Vector? nearest   = null;

    foreach (var v in targets) {
      var d2 = v.Clone().DistanceSquared(src);
      if (d2 >= minDistSq) continue;
      minDistSq = d2;
      nearest   = v;
    }

    return (nearest, MathF.Sqrt(minDistSq));
  }
}