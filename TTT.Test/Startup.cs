using Microsoft.Extensions.DependencyInjection;
using TTT.Api;
using TTT.Api.Events;
using TTT.Api.Player;
using TTT.Game;
using TTT.Plugin;
using TTT.Test.Fakes;

namespace TTT.Test;

public class Startup {
  public void ConfigureServices(IServiceCollection services) {
    services.AddScoped<IEventBus, EventBus>();
    services.AddScoped<IPlayerFinder, FakePlayerFinder>();
    services.AddScoped<FakePlayerFinder>();
    services.AddScoped<IMessenger, FakeMessenger>();
  }
}