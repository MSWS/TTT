using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Game;
using TTT.Game.Listeners;

namespace TTT.CS2.GameHandlers;

public class NameUpdater(IServiceProvider provider) : BaseListener(provider) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  [UsedImplicitly]
  [EventHandler]
  public void OnGameInit(GameInitEvent ev) {
    foreach (var player in Utilities.GetPlayers()) {
      converter.GetPlayer(player).Name = player.PlayerName;
    }
  }
}