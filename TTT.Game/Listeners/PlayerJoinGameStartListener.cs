using GitVersion;
using TTT.Api;
using TTT.Api.Events;
using TTT.Api.Messages;
using TTT.Api.Player;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners;

public class PlayerJoinGameStartListener(IEventBus bus, IPlayerFinder finder,
  IOnlineMessenger messenger, IGameManager games) : IListener, ITerrorModule {
  public void Dispose() { bus.UnregisterListener(this); }

  public string Name => nameof(PlayerJoinGameStartListener);
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { bus.RegisterListener(this); }

  [EventHandler]
  public void OnJoin(PlayerJoinEvent ev) {
    if (games.ActiveGame is not null) return;
    var playerCount = finder.GetAllPlayers().Count;
    if (playerCount < 2) return;

    _ = messenger.MessageAll(finder,
      $"There are {playerCount} players online, starting the game...");

    var game = games.CreateGame();
    game?.Start();
  }
}