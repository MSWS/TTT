using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.Game.Commands;
using TTT.Game.Listeners;
using ICommandManager = TTT.API.Command.ICommandManager;

namespace TTT.Game;

public static class GameServiceCollection {
  public static void AddGameServices(this IServiceCollection collection) {
    collection.AddScoped<IEventBus, EventBus>();
    collection.AddScoped<IGameManager, GameManager>();
    collection.AddScoped<ICommandManager, CommandManager>();

    // Listeners
    collection.AddListener<GameEndLogsListener>();
    collection.AddListener<GamePlayerActionsListener>();
    collection.AddListener<GameRestartListener>();
    collection.AddListener<PlayerCausesEndListener>();
    collection.AddListener<PlayerJoinStarting>();
  }
}