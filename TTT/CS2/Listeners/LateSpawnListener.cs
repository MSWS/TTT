using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.Listeners;

public class LateSpawnListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  [EventHandler]
  public void OnJoin(PlayerJoinEvent ev) {
    if (Games.ActiveGame is { State: State.IN_PROGRESS }) return;

    Server.NextWorldUpdate(() => {
      var player = converter.GetPlayer(ev.Player);
      if (player == null || !player.IsValid) return;
      player.Respawn();
    });
  }
}