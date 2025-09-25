using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Command;
using TTT.CS2.Command.Test;
using TTT.CS2.Game;
using TTT.CS2.GameHandlers;
using TTT.CS2.GameHandlers.DamageCancelers;
using TTT.CS2.Hats;
using TTT.CS2.lang;
using TTT.CS2.Listeners;
using TTT.CS2.Player;
using TTT.Game;
using TTT.Game.Commands;
using TTT.Locale;

namespace TTT.CS2;

public static class CS2ServiceCollection {
  public static void AddCS2Services(this IServiceCollection collection) {
    // TTT - CS2 Specific requirements
    collection
     .AddModBehavior<IPlayerConverter<CCSPlayerController>,
        CCPlayerConverter>();
    collection.AddModBehavior<IStorage<TTTConfig>, CS2GameConfig>();
    collection.AddModBehavior<ICommandManager, CS2CommandManager>();

    // TTT - CS2 Specific optionals
    collection.AddScoped<ITextSpawner, TextSpawner>();

    // GameHandlers
    collection.AddModBehavior<PlayerConnectionsHandler>();
    collection.AddModBehavior<RoundEnd_GameEndHandler>();
    collection.AddModBehavior<RoundStart_GameStartHandler>();
    collection.AddModBehavior<CombatHandler>();
    collection.AddModBehavior<PropMover>();
    collection.AddModBehavior<BodySpawner>();
    collection.AddModBehavior<RoleIconsHandler>();
    collection.AddModBehavior<DamageCanceler>();

    // Damage Cancelers
    collection.AddModBehavior<OutOfRoundCanceler>();
    collection.AddModBehavior<TaserListenCanceler>();

    // Listeners
    collection.AddModBehavior<RoundTimerListener>();
    collection.AddModBehavior<BodyPickupListener>();
    collection.AddModBehavior<PlayerStatsTracker>();

    // Commands
    collection.AddModBehavior<TTTCommand>();
    collection.AddModBehavior<TestCommand>();

    collection.AddScoped<IGameManager, CS2GameManager>();
    collection.AddScoped<IInventoryManager, CS2InventoryManager>();
    collection.AddScoped<IMessenger, CS2Messenger>();
    collection.AddScoped<IMsgLocalizer, StringLocalizer>();
    collection.AddScoped<IPermissionManager, CS2PermManager>();
    collection.AddScoped<IPlayerFinder, CS2PlayerFinder>();
  }
}