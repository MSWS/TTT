namespace TTT.Api.Events.Game;

public abstract class GameEvent(IGame game) : Event {
  public IGame Game { get; } = game;
}