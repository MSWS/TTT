using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Game.Loggers;

public class SimpleLogger(IServiceProvider provider) : IActionLogger {
  private readonly SortedDictionary<DateTime, ISet<IAction>> actions = new();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

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

    if (epoch == null || timestamp < epoch) epoch = timestamp.DateTime;
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
    Server.NextWorldUpdate(() => {
      msg.Value.BackgroundMsgAll(locale[GameMsgs.GAME_LOGS_HEADER]);
      foreach (var (time, action) in GetActions())
        msg.Value.BackgroundMsgAll($"{formatTime(time)} {action.Format()}");
      msg.Value.BackgroundMsgAll(locale[GameMsgs.GAME_LOGS_FOOTER]);
    });
  }

  public void PrintLogs(IOnlinePlayer? player) {
    Server.NextWorldUpdate(() => {
      msg.Value.BackgroundMsg(player, locale[GameMsgs.GAME_LOGS_HEADER]);
      foreach (var (time, action) in GetActions())
        msg.Value.BackgroundMsg(player,
          $"{formatTime(time)} {action.Format()}");
      msg.Value.BackgroundMsg(player, locale[GameMsgs.GAME_LOGS_FOOTER]);
    });
  }

  private string formatTime(DateTime time) {
    if (epoch == null) return time.ToString("o");
    var span = time - epoch.Value;
    return $"[{span.Minutes:D2}:{span.Seconds:D2}]";
  }
}