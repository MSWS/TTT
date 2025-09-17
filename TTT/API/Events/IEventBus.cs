namespace TTT.API.Events;

public interface IEventBus {
  [Obsolete]
  void RegisterListener(IListener listener);
  void UnregisterListener(IListener listener);

  void Dispatch(Event ev);
}