using TTT.API.Player;

namespace TTT.Game.Events.Player;

public class PlayerScreenMessageEvent(IPlayer player, string message)
  : PlayerMessageEvent(player, message) { }