using TTT.API;
using TTT.API.Events;

namespace TTT.Game.Events.Game;

public abstract class GameEvent(IGame game) : Event {
  public IGame Game { get; } = game;
}