using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Configs;
using ShopAPI.Configs.Detective;
using ShopAPI.Configs.Traitor;
using TTT.API.Command;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.API;
using TTT.CS2.Command;
using TTT.CS2.Command.Test;
using TTT.CS2.Configs;
using TTT.CS2.Configs.ShopItems;
using TTT.CS2.Game;
using TTT.CS2.GameHandlers;
using TTT.CS2.GameHandlers.DamageCancelers;
using TTT.CS2.Hats;
using TTT.CS2.lang;
using TTT.CS2.Listeners;
using TTT.CS2.Player;
using TTT.Game;
using TTT.Karma;
using TTT.Locale;

namespace TTT.CS2;

public static class CS2ServiceCollection {
  public static void AddCS2Services(this IServiceCollection collection) {
    // TTT - CS2 Specific requirements
    collection
     .AddModBehavior<IPlayerConverter<CCSPlayerController>,
        CCPlayerConverter>();
    collection.AddModBehavior<ICommandManager, CS2CommandManager>();
    collection.AddModBehavior<IAliveSpoofer, CS2AliveSpoofer>();
    collection.AddModBehavior<IIconManager, RoleIconsHandler>();
    collection.AddModBehavior<NameDisplayer>();
    collection.AddModBehavior<PlayerPingShopAlias>();

    // Configs
    collection.AddModBehavior<IStorage<TTTConfig>, CS2GameConfig>();
    collection.AddModBehavior<IStorage<ShopConfig>, CS2ShopConfig>();
    collection
     .AddModBehavior<IStorage<OneShotDeagleConfig>, CS2OneShotDeagleConfig>();
    collection.AddModBehavior<IStorage<C4Config>, CS2C4Config>();
    collection.AddModBehavior<IStorage<M4A1Config>, CS2M4A1Config>();
    collection.AddModBehavior<IStorage<TaserConfig>, CS2TaserConfig>();
    collection
     .AddModBehavior<IStorage<PoisonSmokeConfig>, CS2PoisonSmokeConfig>();
    collection.AddModBehavior<IStorage<KarmaConfig>, CS2KarmaConfig>();
    collection.AddModBehavior<IStorage<CamoConfig>, CS2CamoConfig>();
    collection.AddModBehavior<IStorage<StickersConfig>, CS2StickersConfig>();
    collection.AddModBehavior<IStorage<BodyPaintConfig>, CS2BodyPaintConfig>();
    collection
     .AddModBehavior<IStorage<DnaScannerConfig>, CS2DnaScannerConfig>();
    collection
     .AddModBehavior<IStorage<HealthStationConfig>, CS2HealthStationConfig>();
    collection
     .AddModBehavior<IStorage<ClusterGrenadeConfig>, CS2ClusterGrenadeConfig>();
    collection.AddModBehavior<IStorage<GlovesConfig>, CS2GlovesConfig>();
    collection
     .AddModBehavior<IStorage<OneHitKnifeConfig>, CS2OneHitKnifeConfig>();
    collection.AddModBehavior<IStorage<SilentAWPConfig>, CS2SilentAWPConfig>();

    // TTT - CS2 Specific optionals
    collection.AddScoped<ITextSpawner, TextSpawner>();

    // GameHandlers
    collection.AddModBehavior<BodySpawner>();
    collection.AddModBehavior<CombatHandler>();
    collection.AddModBehavior<DamageCanceler>();
    collection.AddModBehavior<PlayerConnectionsHandler>();
    collection.AddModBehavior<PropMover>();
    collection.AddModBehavior<RoundStart_GameStartHandler>();
    collection.AddModBehavior<BombPlantSuppressor>();
    collection.AddModBehavior<MapZoneRemover>();
    collection.AddModBehavior<BuyMenuHandler>();
    collection.AddModBehavior<TeamChangeHandler>();
    collection.AddModBehavior<TraitorChatHandler>();
    collection.AddModBehavior<PlayerMuter>();
    collection.AddModBehavior<MapChangeCausesEndListener>();
    // collection.AddModBehavior<EntityTargetHandlers>();

    // Damage Cancelers
    collection.AddModBehavior<OutOfRoundCanceler>();
    collection.AddModBehavior<TaserListenCanceler>();

    // Listeners
    collection.AddModBehavior<AfkTimerListener>();
    collection.AddModBehavior<BodyPickupListener>();
    collection.AddModBehavior<IBodyTracker, BodyTracker>();
    collection.AddModBehavior<LateSpawnListener>();
    collection.AddModBehavior<PlayerStatsTracker>();
    collection.AddModBehavior<RoundTimerListener>();
    collection.AddModBehavior<ScreenColorApplier>();
    collection.AddModBehavior<KarmaBanner>();
    collection.AddModBehavior<KarmaSyncer>();
    collection.AddModBehavior<MapHookListener>();

    // Commands
    collection.AddModBehavior<TestCommand>();

    collection.AddScoped<IGameManager, CS2GameManager>();
    collection.AddScoped<IInventoryManager, CS2InventoryManager>();
    collection.AddScoped<IMessenger, CS2Messenger>();
    collection.AddScoped<IMsgLocalizer, StringLocalizer>();
    collection.AddScoped<IPermissionManager, CS2PermManager>();
    collection.AddScoped<IPlayerFinder, CS2PlayerFinder>();
  }
}