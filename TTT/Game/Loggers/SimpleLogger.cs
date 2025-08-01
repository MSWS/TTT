using System.Reactive.Concurrency;
using TTT.API.Game;

namespace TTT.Game.Loggers;

public class SimpleLogger(IScheduler timer) : IActionLogger {
  private readonly SortedDictionary<DateTime, ISet<IAction>> actions = new();

  public void LogAction(IAction action) {
    var timestamp = timer.Now;
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