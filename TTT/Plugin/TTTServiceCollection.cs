using System.Reactive.Concurrency;
using CounterStrikeSharp.API.Core;
using TTT.CS2;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.Game;

namespace Plugin;

public class TTTServiceCollection : IPluginServiceCollection<TTT> {
  public void ConfigureServices(IServiceCollection serviceCollection) {
    serviceCollection.AddScoped<IEventBus, EventBus>();
    serviceCollection.AddScoped<IScheduler>(_ => Scheduler.Default);

    serviceCollection.AddCS2Services();
  }
}