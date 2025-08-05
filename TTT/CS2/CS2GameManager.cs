using TTT.API.Game;
using TTT.Game;
using TTT.Game.Events.Game;

namespace TTT.CS2;

public class CS2GameManager(IServiceProvider provider) : GameManager(provider) {
  public override IGame CreateGame() {
    ActiveGame = new CS2Game(provider);

    var ev = new GameInitEvent(ActiveGame);
    Bus.Dispatch(ev);

    return ActiveGame;
  }
}