using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game;

namespace TTT.CS2;

public static class CS2ServiceCollection {
  public static void AddCS2Services(this IServiceCollection collection) {
    collection
     .AddScoped<IPlayerConverter<CCSPlayerController>, CCPlayerConverter>();
    collection.AddScoped<IPlayerFinder, CS2PlayerFinder>();
    collection.AddScoped<IStorage<GameConfig>, CS2GameConfig>();
    collection.AddScoped<CS2CommandManager>();
    collection.AddPluginBehavior<CS2GameConfig>();
  }
}