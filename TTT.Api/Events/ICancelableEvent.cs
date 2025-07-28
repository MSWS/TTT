namespace TTT.Api.Events;

public interface ICancelableEvent {
  public bool IsCanceled { get; set; }
}