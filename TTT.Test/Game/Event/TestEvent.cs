using TTT.Api.Events;

namespace TTT.Test.Game.Event;

public class TestEvent : Api.Events.Event, ICancelableEvent {
  public override string Id => "test.event.testevent";
  public bool IsCanceled { get; set; } = false;
}