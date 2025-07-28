using TTT.Api;

namespace TTT.Game.Events.Game;

public class GameStartEvent(IGame game) : GameEvent(game) {
  public override string Id => "core.event.game.start";
}