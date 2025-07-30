using TTT.API.Events;

namespace TTT.Test.Game.Event;

public class TestEvent : TTT.API.Events.Event, ICancelableEvent {
  public override string Id => "test.event.testevent";
  public bool IsCanceled { get; set; } = false;
}