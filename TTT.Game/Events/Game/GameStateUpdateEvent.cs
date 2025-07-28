using TTT.Api;
using TTT.Api.Events;

namespace TTT.Game.Events.Game;

public class GameStateUpdateEvent(IGame game, RoundBasedGame.State newState)
  : GameEvent(game), ICancelableEvent {
  public override string Id => "basegame.event.game.update";
  public RoundBasedGame.State NewState { get; } = newState;
  public bool IsCanceled { get; set; } = false;
}