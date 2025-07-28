using TTT.Api.Events;

namespace TTT.Test.Core.Event;

public class SingleEventListener(IEventBus bus) : IListener {
  public int fired { get; private set; } = 0;

  public void Dispose() => bus.UnregisterListener(this);

  [EventHandler]
  public void OnTestEvent(TestEvent e) { fired++; }
}