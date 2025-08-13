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
  }

  public IEnumerable<(DateTime, IAction)> GetActions() {
    return from kvp in actions
      from action in kvp.Value
      select (kvp.Key, action);
  }

  public void ClearActions() { actions.Clear(); }

  public void PrintLogs() {
    foreach (var (time, action) in GetActions())
      msg.Value.BackgroundMsgAll($"[{time}] {action.Format()}");
  }

  public void PrintLogs(IOnlinePlayer? player) {
    foreach (var (time, action) in GetActions())
      msg.Value.BackgroundMsg(player, $"[{time}] {action.Format()}");
  }
}