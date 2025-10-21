using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners;

public class PlayerJoinStarting(IServiceProvider provider)
  : BaseListener(provider) {
  private TTTConfig config =>
    Provider.GetService<IStorage<TTTConfig>>()?.Load().GetAwaiter().GetResult()
    ?? new TTTConfig();

  [EventHandler]
  [UsedImplicitly]
  public void OnJoin(PlayerJoinEvent ev) {
    if (Games.ActiveGame is { State: State.IN_PROGRESS or State.COUNTDOWN })
      return;
    var playerCount = Finder.GetOnline().Count;
    if (playerCount < config.RoundCfg.MinimumPlayers) return;

    Messenger.DebugInform(
      $"There are {playerCount} Players online, starting the game...");

    var game = Games.CreateGame();

    if (game == null)
      Messenger.DebugAnnounce("Failed to create a new game instance.");
    game?.Start(config.RoundCfg.CountDownDuration);
  }
}