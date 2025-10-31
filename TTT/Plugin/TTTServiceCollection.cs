using System.Reactive.Concurrency;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using SpecialRound;
using Stats;
using TTT.CS2;
using TTT.Game;
using TTT.Karma;
using TTT.RTD;
using TTT.Shop;

namespace TTT.Plugin;

public class TTTServiceCollection : IPluginServiceCollection<TTT> {
  public void ConfigureServices(IServiceCollection serviceCollection) {
    serviceCollection.AddScoped<IScheduler>(_ => Scheduler.Default);

    serviceCollection.AddKarmaService();
    serviceCollection.AddGameServices();
    serviceCollection.AddCS2Services();
    serviceCollection.AddShopServices();
    serviceCollection.AddRtdServices();
    serviceCollection.AddSpecialRounds();

    if (Environment.GetEnvironmentVariable("TTT_STATS_API_URL") == null) return;
    serviceCollection.AddStatsServices();
  }
}