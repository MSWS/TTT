using JetBrains.Annotations;
using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners;

public class PlayerJoinStarting(IServiceProvider provider)
  : BaseListener(provider) {
  public override string Name => nameof(PlayerJoinStarting);

  [EventHandler]
  [UsedImplicitly]
  public void OnJoin(PlayerJoinEvent ev) {
    if (Games.IsGameActive()) return;
    var playerCount = Finder.GetOnline().Count;
    if (playerCount < 2) return;

    _ = Messenger.MessageAll(Finder,
      $"There are {playerCount} players online, starting the game...");

    var game = Games.CreateGame();
    game?.Start(TimeSpan.FromSeconds(5));
  }
}