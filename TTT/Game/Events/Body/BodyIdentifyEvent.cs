using TTT.API.Events;

namespace TTT.Game.Events.Body;

public class BodyIdentifyEvent(IBody body) : BodyEvent(body), ICancelableEvent {
  public override string Id => "base.game.body.identify";
  public bool IsCanceled { get; set; }
}