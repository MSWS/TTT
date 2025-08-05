using TTT.API.Game;
using TTT.Game;

namespace TTT.CS2;

public class CS2GameManager(IServiceProvider provider) : GameManager(provider) {
  public override IGame CreateGame() {
    // var result = base.CreateGame();
    // result.Roles.Add(new SpectatorRole(Provider));
    // result.Roles.Add(new CS2InnocentRole(Provider));
    // result.Roles.Add(new CS2TraitorRole(Provider));
    // result.Roles.Add(new CS2DetectiveRole(Provider));
    // return result;
    return new CS2Game(Provider);
  }
}