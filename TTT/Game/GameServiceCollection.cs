using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Extensions;
using TTT.API.Role;
using TTT.Game.Commands;
using TTT.Game.Listeners;
using TTT.Game.Listeners.Loggers;
using TTT.Game.Roles;

namespace TTT.Game;

public static class GameServiceCollection {
  public static void AddGameServices(this IServiceCollection collection) {
    collection.AddModBehavior<IEventBus, EventBus>();
    collection.AddScoped<IRoleAssigner, RoleAssigner>();

    // Listeners
    collection.AddModBehavior<GameEndLogsListener>();
    collection.AddModBehavior<PlayerCausesEndListener>();
    collection.AddModBehavior<PlayerJoinStarting>();
    collection.AddModBehavior<PlayerActionsLogger>();
    collection.AddModBehavior<BodyIdentifyLogger>();

    // Commands
    collection.AddModBehavior<TTTCommand>();
    collection.AddModBehavior<LogsCommand>();
  }
}