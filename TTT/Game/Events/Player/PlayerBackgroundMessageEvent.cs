using TTT.API.Player;

namespace TTT.Game.Events.Player;

public class PlayerBackgroundMessageEvent(IPlayer player, string message)
  : PlayerMessageEvent(player, message) { }