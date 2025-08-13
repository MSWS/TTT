using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;

namespace TTT.Game.Loggers;

public class SimpleLogger(IServiceProvider provider) : IActionLogger {
  private readonly SortedDictionary<DateTime, ISet<IAction>> actions = new();

  private readonly Lazy<IMessenger> msg =
    new(provider.GetRequiredService<IMessenger>());

  private readonly IScheduler scheduler = provider
   .GetRequiredService<IScheduler>();

  private DateTime? epoch;

  public void LogAction(IAction action) {
#if DEBUG
    msg.Value.Debug(
      $"Logging action: {action.GetType().Name} at {scheduler.Now}");
#endif
    var timestamp = scheduler.Now;
    actions.TryGetValue(timestamp.Date, out var actionSet);
    actionSet ??= new HashSet<IAction>();
    actionSet.Add(action);
    actions[timestamp.DateTime] = actionSet;

    if (epoch == null || timestamp > epoch) epoch = timestamp.DateTime;
  }

  public IEnumerable<(DateTime, IAction)> GetActions() {
    return from kvp in actions
      from action in kvp.Value
      select (kvp.Key, action);
  }

  public void ClearActions() {
    actions.Clear();
    epoch = null;
  }

  public void PrintLogs() {
    foreach (var (time, action) in GetActions())
      msg.Value.BackgroundMsgAll($"{formatTime(time)} {action.Format()}");
  }

  public void PrintLogs(IOnlinePlayer? player) {
    foreach (var (time, action) in GetActions())
      msg.Value.BackgroundMsg(player, $"{formatTime(time)} {action.Format()}");
  }

  private string formatTime(DateTime time) {
    if (epoch == null) return time.ToString("o");
    var span = time - epoch.Value;
    return $"[{span.Minutes:D2}:{span.Seconds:D2}]";
  }
}