namespace TTT.API.Events;

public interface IEventBus {
  [Obsolete("Registration should be done via the ServiceProvider")]
  void RegisterListener(IListener listener);

  void UnregisterListener(IListener listener);

  Task Dispatch(Event ev);
}