using TTT.API.Player;

namespace TTT.Game.Events.Player;

public class PlayerBackgroundMessageEvent(IPlayer player, string message, params object[] args)
  : PlayerMessageEvent(player, message, args) { }