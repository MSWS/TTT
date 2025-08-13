using TTT.API.Events;
using TTT.Game.Actions;
using TTT.Game.Events.Body;

namespace TTT.Game.Listeners.Loggers;

public class BodyIdentifyLogger(IServiceProvider provider)
  : BaseListener(provider) {
  public override string Name => nameof(BodyIdentifyLogger);

  [EventHandler]
  public void OnBodyIdentify(BodyIdentifyEvent ev) {
    if (!Games.IsGameActive()) return;

    var game = Games.ActiveGame;
    if (game == null)
      throw new InvalidOperationException(
        "Active game is null, but game is active?");

    game.Logger.LogAction(new IdentifyBodyAction(ev));
  }
}