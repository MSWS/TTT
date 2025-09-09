namespace TTT.API.Events;

public interface IEventBus {
  void RegisterListener(IListener listener);
  void UnregisterListener(IListener listener);

  Task Dispatch(Event ev);
}