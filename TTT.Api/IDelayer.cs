namespace TTT.Api;

public interface IDelayer {
  Task DelayAsync(TimeSpan delay);
}