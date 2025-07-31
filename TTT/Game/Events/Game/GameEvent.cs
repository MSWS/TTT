using TTT.API.Events;
using TTT.API.Game;

namespace TTT.Game.Events.Game;

public abstract class GameEvent(IGame game) : Event {
  public IGame Game { get; } = game;
}