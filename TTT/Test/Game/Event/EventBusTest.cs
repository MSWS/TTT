using TTT.API.Events;
using Xunit;

namespace TTT.Test.Game.Event;

public class EventBusTest(IEventBus bus) {
  [Fact]
  public void Dispatch_Single_TriggersListener() {
    var listener = new SingleEventListener(bus);
    bus.RegisterListener(listener);
    bus.Dispatch(new TestEvent());

    Assert.Equal(1, listener.fired);
  }

  [Fact]
  public void Dispatch_Multiple_TriggersListener() {
    var listener = new SingleEventListener(bus);
    bus.RegisterListener(listener);
    bus.RegisterListener(listener);
    bus.Dispatch(new TestEvent());

    Assert.Equal(2, listener.fired);
  }

  [Fact]
  public void UnregisterListener_RemovesListener() {
    var listener = new SingleEventListener(bus);
    bus.RegisterListener(listener);
    bus.UnregisterListener(listener);
    bus.Dispatch(new TestEvent());

    Assert.Equal(0, listener.fired);
  }

  [Fact]
  public void Dispatch_Priority_TriggersListenersInOrder() {
    var listener = new PriorityEventListener(bus);
    bus.RegisterListener(listener);
    bus.Dispatch(new TestEvent());

    Assert.Equal([Priority.HIGH, Priority.DEFAULT, Priority.LOW],
      listener.FireOrders);
  }

  [Fact]
  public void Dispatch_CancelableEvent_CancelsEvent() {
    var listener = new CancelableEventListener(bus, true);
    bus.RegisterListener(listener);
    var ev = new TestEvent();
    bus.Dispatch(ev);

    Assert.True(ev.IsCanceled);
    Assert.Equal(1, listener.Fired);
  }

  [Fact]
  public void Dispatch_CancelableEvent_UncanceledWorks() {
    var listener = new CancelableEventListener(bus, false);
    bus.RegisterListener(listener);
    var ev = new TestEvent();
    bus.Dispatch(ev);

    Assert.False(ev.IsCanceled);
    Assert.Equal(2, listener.Fired);
  }
}