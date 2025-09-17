namespace TTT.API.Events;

public interface IEventBus {
  [Obsolete]
  void RegisterListener(IListener listener);
  void UnregisterListener(IListener listener);

  Task Dispatch(Event ev);
}