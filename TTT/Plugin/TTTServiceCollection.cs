using System.Reactive.Concurrency;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.CS2;
using TTT.Game;
using TTT.Karma;

namespace TTT.Plugin;

public class TTTServiceCollection : IPluginServiceCollection<TTT> {
  public void ConfigureServices(IServiceCollection serviceCollection) {
    serviceCollection.AddScoped<IScheduler>(_ => Scheduler.Default);

    serviceCollection.AddKarmaService();
    serviceCollection.AddGameServices();
    serviceCollection.AddCS2Services();
  }
}