using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using TTT.API;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Game;
using TTT.Game.Roles;
using TTT.Test.Fakes;

namespace TTT.Test;

public class Startup {
  public void ConfigureServices(IServiceCollection services) {
    services.AddScoped<IEventBus, EventBus>();
    services.AddScoped<IPlayerFinder, FakePlayerFinder>();
    services.AddScoped<FakePlayerFinder>();
    services.AddScoped<FakeMessenger>();
    services.AddScoped<IOnlineMessenger>(s
      => s.GetRequiredService<FakeMessenger>());
    services.AddScoped<IMessenger>(s => s.GetRequiredService<FakeMessenger>());
    services.AddScoped<IRoleAssigner, RoleAssigner>();
    services.AddScoped<TestScheduler>();
    services.AddScoped<IScheduler>(s => s.GetRequiredService<TestScheduler>());
    services.AddScoped<IGameManager, GameManager>();
  }
}