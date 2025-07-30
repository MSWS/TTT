namespace TTT.Api.Events;

public interface IEventBus {
  void RegisterListener(IListener listener);
  void UnregisterListener(IListener listener);

  void Dispatch(Event ev);
}