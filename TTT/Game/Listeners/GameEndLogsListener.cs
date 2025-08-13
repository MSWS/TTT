using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Events.Game;

namespace TTT.Game.Listeners;

public class GameEndLogsListener(IServiceProvider provider)
  : BaseListener(provider) {
  public override string Name => nameof(GameEndLogsListener);

  [EventHandler(IgnoreCanceled = true, Priority = Priority.MONITOR)]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    ev.Game.Logger.PrintLogs();
  }
}