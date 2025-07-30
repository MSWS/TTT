using TTT.API.Player;

namespace TTT.Game.Events.Player;

/// <summary>
///   Indicates that a player has left the server.
///   A game is not necessarily in progress when this event is fired.
/// </summary>
public class PlayerLeaveEvent(IPlayer player) : PlayerEvent(player) {
  public override string Id => "basegame.event.player.leave";
}