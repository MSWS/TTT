using TTT.API.Events;

namespace TTT.Test.Game.Event;

public class SingleEventListener(IEventBus bus) : IListener {
  public int fired { get; private set; }

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler]
  public void OnTestEvent(TestEvent e) { fired++; }
}