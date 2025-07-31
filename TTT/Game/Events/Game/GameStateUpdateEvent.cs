using TTT.API.Events;
using TTT.API.Game;

namespace TTT.Game.Events.Game;

public class GameStateUpdateEvent(IGame game, State newState)
  : GameEvent(game), ICancelableEvent {
  public override string Id => "basegame.event.game.update";
  public State NewState { get; } = newState;
  public bool IsCanceled { get; set; } = false;
}