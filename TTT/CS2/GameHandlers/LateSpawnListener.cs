using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.GameHandlers;

public class LateSpawnListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  [UsedImplicitly]
  [EventHandler]
  public void OnJoin(PlayerJoinEvent ev) {
    if (Games.ActiveGame is { State: State.IN_PROGRESS }) return;

    Server.NextWorldUpdate(() => {
      var player = converter.GetPlayer(ev.Player);
      if (player == null || !player.IsValid) return;
      player.Respawn();
    });
  }

  [UsedImplicitly]
  [EventHandler]
  public void GameState(GameStateUpdateEvent ev) {
    if (ev.NewState == State.FINISHED) return;

    Server.NextWorldUpdate(() => {
      foreach (var player in Utilities.GetPlayers()
       .Where(p => p.GetHealth() <= 0 && p.Team != CsTeam.Spectator
          && p.Team != CsTeam.None))
        player.Respawn();
    });
  }
}