using TTT.Api.Events;

namespace TTT.Test.Game.Event;

public class PriorityEventListener(IEventBus bus) : IListener {
  private readonly List<uint> fireOrders = [];
  public List<uint> FireOrders => [..fireOrders];

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler(Priority = Priority.HIGH)]
  public void OnEventHighPriority(TestEvent e) {
    fireOrders.Add(Priority.HIGH);
  }

  [EventHandler(Priority = Priority.LOW)]
  public void OnEventLowPriority(TestEvent e) { fireOrders.Add(Priority.LOW); }

  [EventHandler(Priority = Priority.DEFAULT)]
  public void OnEvent(TestEvent e) { fireOrders.Add(Priority.DEFAULT); }
}