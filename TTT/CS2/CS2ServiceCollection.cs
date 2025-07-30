using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;

namespace TTT.CS2;

public static class CS2ServiceCollection {
  public static void AddCS2Services(this IServiceCollection collection) {
    collection
     .AddScoped<IPlayerConverter<CCSPlayerController>, CCPlayerConverter>();
    collection.AddScoped<IPlayerFinder, CS2PlayerFinder>();
  }
}