using TTT.Api.Player;

namespace TTT.Api.Events.Player;

/// <summary>
/// Indicates that a player has joined the server.
/// A game is not necessarily in progress when this event is fired.
/// </summary>
/// <param name="player"></param>
public class PlayerJoinEvent(IPlayer player) : PlayerEvent(player) {
  public override string Id => "core.event.player.join";
}