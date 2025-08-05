using TTT.API.Game;
using TTT.Game;

namespace TTT.CS2;

public class CS2GameManager(IServiceProvider provider) : GameManager(provider) {
  public override IGame CreateGame() { return new CS2Game(Provider); }
}