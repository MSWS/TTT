using TTT.API.Game;
using TTT.Game;

namespace TTT.CS2;

public class CS2GameManager(IServiceProvider provider) : GameManager(provider) {
  public override IGame CreateGame() {
    var result = base.CreateGame();
    result.Roles.Insert(0, new SpectatorRole(Provider));
    return result;
  }
}