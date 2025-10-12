namespace TTT.API.Events;

public class Event {
  public virtual string Id => GetType().Name.ToLowerInvariant();
}