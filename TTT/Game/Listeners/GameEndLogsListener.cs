using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Game.Events.Game;

namespace TTT.Game.Listeners;

public class GameEndLogsListener(IServiceProvider provider)
  : BaseListener(provider) {
  public override string Name => nameof(GameEndLogsListener);

  [EventHandler(IgnoreCanceled = true, Priority = Priority.LOW)]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    var logs = ev.Game.Logger.GetActions();
    foreach (var (timestamp, action) in logs)
      _ = Messenger.BackgroundMsgAll(Finder,
        $"[{timestamp}] {action.Format()}");
  }
}