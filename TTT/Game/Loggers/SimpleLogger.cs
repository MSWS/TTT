using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Game;
using TTT.API.Messages;

namespace TTT.Game.Loggers;

public class SimpleLogger(IServiceProvider provider) : IActionLogger {
  private readonly IMessenger msg = provider.GetRequiredService<IMessenger>();

  private readonly IScheduler scheduler = provider
   .GetRequiredService<IScheduler>();

  private readonly SortedDictionary<DateTime, ISet<IAction>> actions = new();

  public void LogAction(IAction action) {
#if DEBUG
    msg.Debug($"Logging action: {action.GetType().Name} at {scheduler.Now}");
#endif
    var timestamp = scheduler.Now;
    actions.TryGetValue(timestamp.Date, out var actionSet);
    actionSet ??= new HashSet<IAction>();
    actionSet.Add(action);
    actions[timestamp.Date] = actionSet;
  }

  public IEnumerable<(DateTime, IAction)> GetActions() {
    return from kvp in actions
      from action in kvp.Value
      select (kvp.Key, action);
  }

  public void ClearActions() { actions.Clear(); }
}