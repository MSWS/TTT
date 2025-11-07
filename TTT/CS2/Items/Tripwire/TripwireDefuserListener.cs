using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.API.Items;
using TTT.CS2.Extensions;
using TTT.CS2.RayTrace.Class;
using TTT.CS2.RayTrace.Enum;
using TTT.Locale;

namespace TTT.CS2.Items.Tripwire;

public class TripwireDefuserListener(IServiceProvider provider)
  : IPluginModule {
  private readonly ITripwireTracker? tripwireTracker =
    provider.GetService<ITripwireTracker>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private TripwireConfig config
    => provider.GetService<IStorage<TripwireConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new TripwireConfig();

  public void Dispose() { }
  public void Start() { }

  public void Start(BasePlugin? plugin) {
    if (tripwireTracker == null) return;
    plugin
    ?.RegisterListener<
        CounterStrikeSharp.API.Core.Listeners.OnPlayerButtonsChanged>(
        onButtonsChanged);
  }

  private void onButtonsChanged(CCSPlayerController player,
    PlayerButtons pressed, PlayerButtons released) {
    if (tripwireTracker == null) return;
    if ((pressed & PlayerButtons.Use) != PlayerButtons.Use) return;

    var instance = getTargetTripwire(player);
    if (instance == null) return;

    startDefuseTimer(player, instance);
  }

  private TripwireInstance? getTargetTripwire(CCSPlayerController player) {
    if (tripwireTracker == null) return null;
    var raytrace =
      player.GetGameTraceByEyePosition(TraceMask.MaskSolid, Contents.NoDraw,
        player);

    if (raytrace == null || tripwireTracker.ActiveTripwires.Count == 0)
      return null;
    var raytracePos = raytrace.Value.EndPos.toVector();

    var closest =
      tripwireTracker?.ActiveTripwires.MinBy(i
        => i.StartPos.DistanceSquared(raytracePos));

    if (closest == null || closest.StartPos.DistanceSquared(raytracePos)
      > config.TripwireSizeSquared)
      return null;

    return closest;
  }

  private void startDefuseTimer(CCSPlayerController player,
    TripwireInstance instance) {
    tickDefuse(player, instance, DateTime.Now);
  }

  private void tickDefuse(CCSPlayerController player, TripwireInstance instance,
    DateTime startTime) {
    if (!player.IsValid) return;
    var apiPlayer = converter.GetPlayer(player);

    if ((player.Buttons & PlayerButtons.Use) != PlayerButtons.Use) {
      messenger.Message(apiPlayer,
        locale[TripwireMsgs.SHOP_ITEM_TRIPWIRE_DEFUSING_CANCELED]);
      return;
    }

    var progress = (DateTime.Now - startTime) / config.DefuseTime;
    var timeLeft = config.DefuseTime - (config.DefuseTime * progress);

    if (progress >= 1) {
      tripwireTracker?.RemoveTripwire(instance);
      return;
    }

    var target = getTargetTripwire(player);
    if (target != instance) {
      messenger.Message(apiPlayer,
        locale[TripwireMsgs.SHOP_ITEM_TRIPWIRE_DEFUSING_CANCELED]);
      return;
    }

    player.PrintToCenter(
      locale[TripwireMsgs.SHOP_ITEM_TRIPWIRE_DEFUSING(progress, timeLeft)]);
    var ticksDelay = (int)Math.Round(64 * config.DefuseRate.TotalSeconds);
    Server.RunOnTick(Server.TickCount + ticksDelay,
      () => tickDefuse(player, instance, startTime));
  }
}