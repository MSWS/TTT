using TTT.Api;
using TTT.Api.Events;

namespace TTT.Game.Events.Game;

public abstract class GameEvent(IGame game) : Event {
  public IGame Game { get; } = game;
}