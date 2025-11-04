using System.Drawing;
using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.CS2.RayTrace.Class;
using TTT.CS2.RayTrace.Enum;
using TTT.Game.Roles;

namespace TTT.CS2.Items.Tripwire;

public static class TripwireServiceCollection {
  public static void AddTripwireServices(this IServiceCollection services) {
    services.AddModBehavior<TripwireItem>();
    services.AddModBehavior<TripwireMovementListener>();
  }
}

public class TripwireItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider), IPluginModule {
  public override string Name => Locale[TripwireMsgs.SHOP_ITEM_TRIPWIRE];

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  protected readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public record TripwireInstance(IOnlinePlayer owner, CEnvBeam Beam,
    CDynamicProp TripwireProp, Vector StartPos, Vector EndPos);

  private TripwireConfig config = provider
   .GetService<IStorage<TripwireConfig>>()
  ?.Load()
   .GetAwaiter()
   .GetResult() ?? new TripwireConfig();

  public override string Description
    => Locale[TripwireMsgs.SHOP_ITEM_TRIPWIRE_DESC];

  public override ShopItemConfig Config => config;

  public List<TripwireInstance> ActiveTripwires = new();

  public void Start(BasePlugin? plugin) {
    Start();
    plugin
    ?.RegisterListener<
        CounterStrikeSharp.API.Core.Listeners.OnServerPrecacheResources>(
        onPrecache);
  }

  private void onPrecache(ResourceManifest manifest) {
    manifest.AddResource(
      "models/generic/conveyor_control_panel_01/conveyor_button_02.vmdl");
  }

  public override void OnPurchase(IOnlinePlayer player) {
    Server.NextWorldUpdate(() => {
      var gamePlayer = converter.GetPlayer(player);
      var playerPawn = gamePlayer?.PlayerPawn.Value;
      if (gamePlayer == null || playerPawn == null) return;
      var originTrace =
        gamePlayer.GetGameTraceByEyePosition(TraceMask.MaskSolid,
          Contents.NoDraw, gamePlayer);
      var origin = gamePlayer.GetEyePosition();
      if (origin == null || originTrace == null) return;

      var angles = vectorToAngle(originTrace.Value.Normal.toVector());

      var endTrace = TraceRay.TraceShape(origin, angles, TraceMask.MaskSolid,
        Contents.NoDraw, gamePlayer);

      var tripwire = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic");
      if (tripwire == null) return;

      tripwire.SetModel(
        "models/generic/conveyor_control_panel_01/conveyor_button_02.vmdl");
      tripwire.DispatchSpawn();

      tripwire.Teleport(originTrace.Value.EndPos.toVector(),
        vectorToAngle(originTrace.Value.Normal.toVector()));
      tripwire.EmitSound("Weapon_ELITE.Clipout");

      scheduler.Schedule(TimeSpan.FromSeconds(2), () => {
        Server.NextWorldUpdate(() => {
          if (!gamePlayer.IsValid) return;
          createBeam(player, tripwire, originTrace.Value.EndPos.toVector(),
            endTrace.EndPos.toVector());
        });
      });
    });
  }

  private void createBeam(IOnlinePlayer owner, CDynamicProp prop, Vector start,
    Vector end) {
    prop.EmitSound("C4.ExplodeTriggerTrip");
    var beam = createBeamEnt(start, end);
    if (beam == null) return;

    var instance = new TripwireInstance(owner, beam, prop, start, end);
    ActiveTripwires.Add(instance);
  }

  private QAngle vectorToAngle(Vector vec) {
    var pitch = (float)(Math.Atan2(-vec.Z,
      Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y)) * (180.0 / Math.PI));
    var yaw = (float)(Math.Atan2(vec.Y, vec.X) * (180.0 / Math.PI));
    return new QAngle(pitch, yaw, 0);
  }

  private CEnvBeam? createBeamEnt(Vector start, Vector end) {
    var beam = Utilities.CreateEntityByName<CEnvBeam>("env_beam");
    if (beam == null) return null;
    beam.RenderMode = RenderMode_t.kRenderTransAlpha;
    beam.Width      = 0.5f;
    beam.Render     = Color.FromArgb(128, Color.Red);
    beam.EndPos.X   = end.X;
    beam.EndPos.Y   = end.Y;
    beam.EndPos.Z   = end.Z;
    beam.Teleport(start);
    return beam;
  }
}