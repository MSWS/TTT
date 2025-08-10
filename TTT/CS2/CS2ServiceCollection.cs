using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Extensions;
using TTT.API.Game;
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
    // Base Requirements
    collection.AddScoped<IGameManager, CS2GameManager>();

    // TTT - CS2 Specific requirements
    collection
     .AddModBehavior<IPlayerConverter<CCSPlayerController>,
        CCPlayerConverter>();
    collection.AddScoped<IPlayerFinder, CS2PlayerFinder>();
    collection.AddModBehavior<IStorage<GameConfig>, CS2GameConfig>();
    collection.AddPluginBehavior<ICommandManager, CS2CommandManager>();
    collection.AddScoped<IMessenger, CS2Messenger>();
    collection.AddScoped<IInventoryManager, CS2InventoryManager>();

    // GameHandlers
    collection.AddPluginBehavior<PlayerConnectionsHandler>();
    collection.AddPluginBehavior<RoundEndHandler>();
    collection.AddPluginBehavior<RoundStartHandler>();
    collection.AddPluginBehavior<CombatHandler>();
    collection.AddPluginBehavior<PropMover>();
    collection.AddPluginBehavior<BodyHider>();

    // Listeners
    collection.AddListener<RoleAssignListener>();
    collection.AddListener<RoundTimerListener>();

    collection.AddScoped<IMsgLocalizer, StringLocalizer>();
    collection.AddScoped<IPermissionManager, CS2PermManager>();
  }
}