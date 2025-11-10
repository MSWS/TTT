using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Storage;
using TTT.CS2.API.Items;
using TTT.CS2.Extensions;

namespace TTT.CS2.Items.Tripwire;

public class TripwireDamageListener(IServiceProvider provider) : IPluginModule {
  private readonly ITripwireActivator? tripwireActivator =
    provider.GetRequiredService<ITripwireActivator>();

  private readonly ITripwireTracker? tripwires =
    provider.GetService<ITripwireTracker>();

  private TripwireConfig config
    => provider.GetService<IStorage<TripwireConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new TripwireConfig();

  public void Dispose() { }
  public void Start() { }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnBulletImpact(EventBulletImpact ev, GameEventInfo info) {
    if (tripwires == null) return HookResult.Continue;
    var hitVec = new Vector(ev.X, ev.Y, ev.Z);

    var nearest = tripwires.ActiveTripwires
     .OrderBy(wire => wire.TripwireProp.AbsOrigin.DistanceSquared(hitVec))
     .FirstOrDefault();

    if (nearest == null) return HookResult.Continue;
    var distSquared = nearest.TripwireProp.AbsOrigin.DistanceSquared(hitVec);
    if (distSquared > config.TripwireSizeSquared) return HookResult.Continue;

    tripwireActivator?.ActivateTripwire(nearest);
    return HookResult.Continue;
  }
}