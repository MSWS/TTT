using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Extensions;
using TTT.API.Role;
using TTT.Game.Listeners;
using TTT.Game.Listeners.Loggers;
using TTT.Game.Roles;

namespace TTT.Game;

public static class GameServiceCollection {
  public static void AddGameServices(this IServiceCollection collection) {
    collection.AddScoped<IEventBus, EventBus>();
    collection.AddScoped<IRoleAssigner, RoleAssigner>();

    // Listeners
    collection.AddListener<GameEndLogsListener>();
    collection.AddListener<PlayerCausesEndListener>();
    collection.AddListener<PlayerJoinStarting>();
    collection.AddListener<PlayerActionsLogger>();
    collection.AddListener<BodyIdentifyLogger>();
  }
}