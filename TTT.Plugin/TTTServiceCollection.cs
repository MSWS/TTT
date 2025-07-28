using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.Api.Events;
using TTT.CS2;
using TTT.Game;

namespace TTT.Plugin;

public class TTTServiceCollection : IPluginServiceCollection<TTT> {
  public void ConfigureServices(IServiceCollection serviceCollection) {
    serviceCollection.AddScoped<IEventBus, EventBus>();

    serviceCollection.AddCS2Services();
  }
}