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
  }
}

public class TripwireItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider), IPluginModule {
  public override string Name => Locale[TripwireMsgs.SHOP_ITEM_TRIPWIRE];

  protected readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private TripwireConfig config = provider
   .GetService<IStorage<TripwireConfig>>()
  ?.Load()
   .GetAwaiter()
   .GetResult() ?? new TripwireConfig();

  public override string Description
    => Locale[TripwireMsgs.SHOP_ITEM_TRIPWIRE_DESC];

  public override ShopItemConfig Config => config;

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

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    PurchaseResult result = base.CanPurchase(player);
    if (result != PurchaseResult.SUCCESS) return result;
    Server.NextWorldUpdateAsync(() => {
        var gamePlayer = converter.GetPlayer(player);
        if (gamePlayer == null) {
          result = PurchaseResult.UNKNOWN_ERROR;
          return;
        }

        var trace = gamePlayer.GetGameTraceByEyePosition(TraceMask.MaskSolid,
          Contents.NoDraw, gamePlayer);
        if (trace == null) {
          result = PurchaseResult.TRIPWIRE_TOO_FAR;
          return;
        }

        var endPos = trace.Value.EndPos;
        var distance = gamePlayer.GetEyePosition()
         .DistanceSquared(endPos.toVector());
        if (distance > 2500) {
          result = PurchaseResult.TRIPWIRE_TOO_FAR;
          return;
        }
      })
     .GetAwaiter()
     .GetResult();

    return result;
  }

  public override void OnPurchase(IOnlinePlayer player) {
    Server.NextWorldUpdate(() => {
      var gamePlayer = converter.GetPlayer(player);
      if (gamePlayer == null) return;
      var trace = gamePlayer.GetGameTraceByEyePosition(TraceMask.MaskSolid,
        Contents.NoDraw, gamePlayer);
      if (trace == null) return;

      var tripwire = Utilities.CreateEntityByName<CPhysicsProp>("prop_physics");

      if (tripwire == null) return;

      tripwire.SetModel(
        "models/generic/conveyor_control_panel_01/conveyor_button_02.vmdl");
      tripwire.DispatchSpawn();

      tripwire.Teleport(trace.Value.EndPos.toVector());
    });
  }
}