using TTT.API.Game;
using TTT.CS2.Roles;
using TTT.Game;
using TTT.Game.Roles;

namespace TTT.CS2;

public class CS2GameManager(IServiceProvider provider) : GameManager(provider) {
  public override IGame CreateGame() {
    var result = base.CreateGame();
    result.Roles.Add(new SpectatorRole(Provider));
    result.Roles.Add(new CS2InnocentRole(Provider));
    result.Roles.Add(new CS2TraitorRole(Provider));
    result.Roles.Add(new CS2DetectiveRole(Provider));
    return result;
  }
}