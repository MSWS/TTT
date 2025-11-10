using Microsoft.Extensions.DependencyInjection;
using SpecialRound.Rounds;
using SpecialRoundAPI;
using TTT.API.Extensions;

namespace SpecialRound;

public static class SpecialRoundCollection {
  public static void AddSpecialRounds(this IServiceCollection services) {
    services.AddModBehavior<ISpecialRoundStarter, SpecialRoundStarter>();
    services.AddModBehavior<ISpecialRoundTracker, SpecialRoundTracker>();
    services.AddModBehavior<SpecialRoundSoundNotifier>();

    services.AddModBehavior<BhopRound>();
    services.AddModBehavior<LowGravRound>();
    services.AddModBehavior<PistolRound>();
    services.AddModBehavior<RichRound>();
    services.AddModBehavior<SilentRound>();
    services.AddModBehavior<SpeedRound>();
    services.AddModBehavior<SuppressedRound>();
    services.AddModBehavior<VanillaRound>();
  }
}