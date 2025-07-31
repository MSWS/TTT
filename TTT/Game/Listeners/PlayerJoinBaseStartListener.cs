using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners;

public class PlayerJoinBaseStartListener(IServiceProvider provider)
  : BaseListener(provider) {
  public override string Name => nameof(PlayerJoinBaseStartListener);

  [EventHandler]
  public void OnJoin(PlayerJoinEvent ev) {
    if (Games.ActiveGame is not null) return;
    var playerCount = Finder.GetOnline().Count;
    if (playerCount < 2) return;

    _ = Messenger.MessageAll(Finder,
      $"There are {playerCount} players online, starting the game...");

    var game = Games.CreateGame();
    game?.Start(TimeSpan.FromSeconds(5));
  }
}