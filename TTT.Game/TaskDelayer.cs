using TTT.Api;

namespace TTT.Game;

public class TaskDelayer : IDelayer {
  public Task DelayAsync(TimeSpan delay) { return Task.Delay(delay); }
}