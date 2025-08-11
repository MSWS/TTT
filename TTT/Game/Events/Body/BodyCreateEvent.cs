using TTT.API.Events;

namespace TTT.Game.Events.Body;

public class BodyCreateEvent(IBody body) : BodyEvent(body), ICancelableEvent {
  public override string Id => "base.game.body.create";
  public bool IsCanceled { get; set; }
}