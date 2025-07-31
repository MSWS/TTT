using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Reactive.Testing;
using TTT.API.Events;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.Game;
using TTT.Game.Roles;
using TTT.Locale;
using TTT.Test.Abstract;
using TTT.Test.Fakes;

namespace TTT.Test;

public class Startup {
  public void ConfigureServices(IServiceCollection services) {
    services.AddScoped<IEventBus, EventBus>();
    services.AddScoped<IPlayerFinder, FakePlayerFinder>();
    services.AddScoped<IOnlineMessenger, FakeMessenger>();
    services.AddScoped<IMessenger>(s
      => s.GetRequiredService<IOnlineMessenger>());
    services.AddScoped<IRoleAssigner, RoleAssigner>();
    services.AddScoped<TestScheduler>();
    services.AddScoped<IScheduler>(s => s.GetRequiredService<TestScheduler>());
    services.AddScoped<IGameManager, GameManager>();
    services.AddScoped<IStringLocalizerFactory, JsonLocalizerFactory>();
    services.AddScoped<StringLocalizer>();
    services.AddTransient<IMsgLocalizer>(s
      => s.GetRequiredService<StringLocalizer>());
    services.AddScoped<IInventoryManager, FakeInventoryManager>();
    services.AddTransient<IStorage<GameConfig>, FakeConfig>();

    services.AddModBehavior<GenericInitTester>();
    services.AddModBehavior<PluginInitTester>();
  }
}