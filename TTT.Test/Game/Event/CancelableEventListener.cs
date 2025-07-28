using TTT.Api.Events;

namespace TTT.Test.Core.Event;

public class CancelableEventListener(IEventBus bus, bool cancelEvent)
  : IListener {
  public int Fired { get; private set; }

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler]
  public void OnEvent(TestEvent e) {
    Fired++;
    if (cancelEvent) e.IsCanceled = true;
  }

  [EventHandler(Priority = Priority.LOW, IgnoreCanceled = true)]
  public void AfterOnEvent(TestEvent e) { Fired++; }
}