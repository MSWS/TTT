using TTT.API.Events;

namespace TTT.Game.Events.Body;

public abstract class BodyEvent(IBody body) : Event {
  public IBody Body { get; } = body;
}