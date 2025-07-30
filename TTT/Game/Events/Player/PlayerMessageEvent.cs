using TTT.API.Events;
using TTT.API.Player;

namespace TTT.Game.Events.Player;

public class PlayerMessageEvent(IPlayer player, string message)
  : PlayerEvent(player), ICancelableEvent {
  public override string Id => "basegame.event.player.message";
  public string Message { get; set; } = message;
  public bool IsCanceled { get; set; } = false;
}