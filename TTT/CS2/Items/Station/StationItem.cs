using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.Extensions;
using TTT.CS2.RayTrace.Class;

namespace TTT.CS2.Items.Station;

public abstract class StationItem<T>(IServiceProvider provider,
  StationConfig config)
  : RoleRestrictedItem<T>(provider), IPluginModule where T : IRole {
  protected readonly StationConfig _Config = config;

  protected readonly IPlayerConverter<CCSPlayerController> Converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly long PROP_SIZE_SQUARED = 700;

  protected readonly Dictionary<CPhysicsPropMultiplayer, StationInfo> props =
    new();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private IDisposable? intervalHandle;

  public override ShopItemConfig Config => _Config;

  public override void Start() {
    base.Start();
    intervalHandle = scheduler.SchedulePeriodic(_Config.HealthInterval,
      () => Server.NextWorldUpdate(onInterval));
  }

  public void Start(BasePlugin? plugin) {
    Start();
    plugin
    ?.RegisterListener<
        CounterStrikeSharp.API.Core.Listeners.OnServerPrecacheResources>(m => {
        m.AddResource("models/props/cs_office/microwave.vmdl");
      });
  }

  public override void Dispose() {
    base.Dispose();
    intervalHandle?.Dispose();
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnBulletImpact(EventBulletImpact ev, GameEventInfo info) {
    var hitVec = new Vector(ev.X, ev.Y, ev.Z);

    var nearest = props
     .Select(kv => (kv.Key, kv.Value,
        Distance: kv.Key.AbsOrigin!.DistanceSquared(hitVec)))
     .Where(t => t.Key is { IsValid: true, AbsOrigin: not null })
     .OrderBy(t => t.Distance)
     .FirstOrDefault();

    if (nearest.Key == null || nearest.Value == null
      || nearest.Distance > PROP_SIZE_SQUARED)
      return HookResult.Continue;

    var dmg = getBulletDamage(ev);
    nearest.Value.Health -= dmg;

    if (nearest.Value.Health <= 0) {
      nearest.Key.AcceptInput("Kill");
      props.Remove(nearest.Key);
      return HookResult.Continue;
    }

    nearest.Key.SetColor(
      _Config.GetColor(nearest.Value.Health / (float)_Config.StationHealth));
    return HookResult.Continue;
  }

  private int getBulletDamage(EventBulletImpact ev) {
    var user   = ev.Userid;
    var weapon = user?.Pawn.Value?.WeaponServices?.ActiveWeapon.Value;
    var dist =
      (user?.Pawn.Value?.AbsOrigin?.Distance(new Vector(ev.X, ev.Y, ev.Z))
        ?? null) ?? 1;
    var distScale  = Math.Clamp(256 / dist, 0.1, 1);
    var baseDamage = getBaseDamage(weapon?.DesignerName ?? "");
    var total      = (int)(baseDamage * distScale);
    return Math.Max(total, 1);
  }

  private int getBaseDamage(string designerWeapon) {
    return designerWeapon switch {
      "weapon_awp"                                 => 115,
      "weapon_glock"                               => 8,
      "weapon_usp_silencer"                        => 20,
      "weapon_deagle"                              => 40,
      _ when Tag.PISTOLS.Contains(designerWeapon)  => 10,
      _ when Tag.SMGS.Contains(designerWeapon)     => 15,
      _ when Tag.SHOTGUNS.Contains(designerWeapon) => 15,
      _ when Tag.RIFLES.Contains(designerWeapon)   => 35,
      _                                            => 5
    };
  }

  public override void OnPurchase(IOnlinePlayer player) {
    Server.NextWorldUpdate(() => {
      var prop =
        Utilities.CreateEntityByName<CPhysicsPropMultiplayer>(
          "prop_physics_multiplayer");

      if (prop == null) return;

      props[prop] = new StationInfo(prop, _Config.StationHealth, player);

      prop.SetModel("models/props/cs_office/microwave.vmdl");
      prop.DispatchSpawn();

      var gamePlayer = Converter.GetPlayer(player);
      if (gamePlayer == null || !gamePlayer.Pawn.IsValid
        || gamePlayer.Pawn.Value == null)
        return;

      var spawnPos = gamePlayer.GetEyePosition();
      var forward  = gamePlayer.Pawn.Value.AbsRotation;

      if (spawnPos == null) return;

      if (forward == null) forward = new QAngle(0, 0, 0);

      spawnPos += forward.ToForward() * 50;

      prop.Teleport(spawnPos);
    });
  }

  abstract protected void onInterval();
}