using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Command;
using TTT.CS2.Game;
using TTT.CS2.GameHandlers;
using TTT.CS2.Hats;
using TTT.CS2.lang;
using TTT.CS2.Listeners;
using TTT.CS2.Player;
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
    collection.AddModBehavior<IStorage<TTTConfig>, CS2GameConfig>();
    collection.AddPluginBehavior<ICommandManager, CS2CommandManager>();
    collection.AddScoped<IMessenger, CS2Messenger>();
    collection.AddScoped<IInventoryManager, CS2InventoryManager>();

    // TTT - CS2 Specific optionals
    collection.AddScoped<ITextSpawner, TextSpawner>();

    // GameHandlers
    collection.AddPluginBehavior<PlayerConnectionsHandler>();
    collection.AddPluginBehavior<RoundEndHandler>();
    collection.AddPluginBehavior<RoundStartHandler>();
    collection.AddPluginBehavior<CombatHandler>();
    collection.AddPluginBehavior<PropMover>();
    collection.AddPluginBehavior<BodySpawner>();
    collection.AddPluginBehavior<RoleIconsHandler>();

    // Listeners
    collection.AddListener<RoundTimerListener>();
    collection.AddListener<BodyPickupListener>();

    collection.AddScoped<IMsgLocalizer, StringLocalizer>();
    collection.AddScoped<IPermissionManager, CS2PermManager>();
  }
}