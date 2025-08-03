using TTT.API.Game;

namespace TTT.Game.Events.Game;

public class GameInitEvent(IGame game) : GameEvent(game) {
  public override string Id => "basegame.event.game.init";
}