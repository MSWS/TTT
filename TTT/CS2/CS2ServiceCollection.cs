using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Extensions;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.GameHandlers;
using TTT.CS2.Listeners;
using TTT.Game;
using TTT.Locale;

namespace TTT.CS2;

public static class CS2ServiceCollection {
  public static void AddCS2Services(this IServiceCollection collection) {
    // TTT - CS2 Specific requirements
    collection
     .AddModBehavior<IPlayerConverter<CCSPlayerController>,
        CCPlayerConverter>();
    collection.AddScoped<IPlayerFinder, CS2PlayerFinder>();
    collection.AddModBehavior<IStorage<GameConfig>, CS2GameConfig>();
    collection.AddPluginBehavior<ICommandManager, CS2CommandManager>();
    collection.AddScoped<IMessenger, CS2Messenger>();

    // GameHandlers
    // collection.AddPluginBehavior<CombatHandler>();
    collection.AddPluginBehavior<PlayerConnectionsHandler>();
    collection.AddPluginBehavior<RoundEndHandler>();

    // Listeners
    collection.AddListener<RoleAssignListener>();
    collection.AddListener<RoundTimerListener>();

    collection.AddScoped<IMsgLocalizer, StringLocalizer>();
    collection.AddScoped<IPermissionManager, CS2PermManager>();
  }
}