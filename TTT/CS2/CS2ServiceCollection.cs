using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.GameHandlers;
using TTT.CS2.Listeners;
using TTT.Game;

namespace TTT.CS2;

public static class CS2ServiceCollection {
  public static void AddCS2Services(this IServiceCollection collection) {
    // TTT - CS2 Specific requirements
    collection
     .AddScoped<IPlayerConverter<CCSPlayerController>, CCPlayerConverter>();
    collection.AddScoped<IPlayerFinder, CS2PlayerFinder>();
    collection.AddScoped<IStorage<GameConfig>, CS2GameConfig>();
    collection.AddPluginBehavior<CS2CommandManager>();

    // GameHandlers
    collection.AddPluginBehavior<CombatHandler>();
    collection.AddPluginBehavior<PlayerConnectionsHandler>();
    collection.AddPluginBehavior<RoundEndHandler>();

    // Listeners
    collection.AddListener<RoleAssignListener>();
    collection.AddListener<RoundTimerListener>();
  }
}