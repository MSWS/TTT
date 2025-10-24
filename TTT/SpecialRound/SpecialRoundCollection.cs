using Microsoft.Extensions.DependencyInjection;
using SpecialRoundAPI;
using TTT.API.Extensions;

namespace SpecialRound;

public static class SpecialRoundCollection {
  public static void AddSpecialRounds(this IServiceCollection services) {
    services.AddModBehavior<ISpecialRoundStarter, SpecialRoundStarter>();
    services.AddModBehavior<ISpecialRoundTracker, SpecialRoundTracker>();
  }
}