using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.Api;
using TTT.Api.Player;

namespace TTT.CS2;

public static class CS2ServiceCollection {
  public static void AddCS2Services(this IServiceCollection collection) {
    collection
     .AddScoped<IPlayerConverter<CCSPlayerController>, CCPlayerConverter>();
  }
}