using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;

namespace Stats;

public static class StatsServiceCollection {
  public static void AddStatsSerivces(this IServiceCollection collection) {
    collection.AddModBehavior<PlayerCreationListener>();
  }
}