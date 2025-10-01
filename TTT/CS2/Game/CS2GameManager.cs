using Microsoft.Extensions.DependencyInjection;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.Game;
using TTT.Game.Events.Game;

namespace TTT.CS2.Game;

public class CS2GameManager(IServiceProvider provider) : GameManager(provider) {
  protected readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  public override IGame CreateGame() {
    messenger.Debug("Attempting to create a new CS2 game...");
    switch (ActiveGame) {
      case { State: State.IN_PROGRESS or State.COUNTDOWN }:
        throw new InvalidOperationException(
          "A game is already active. End the current game before starting a new one.");
      case { State: State.WAITING }:
        return ActiveGame;
    }

    messenger.Debug("Creating a new CS2 game instance...");
    ActiveGame = new CS2Game(Provider);

    var ev = new GameInitEvent(ActiveGame);
    Bus.Dispatch(ev);

    return ActiveGame;
  }
}