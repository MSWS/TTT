namespace TTT.API.Events;

public interface ICancelableEvent {
  public bool IsCanceled { get; set; }
}