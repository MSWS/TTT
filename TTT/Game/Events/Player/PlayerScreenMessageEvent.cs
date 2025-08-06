using TTT.API.Player;

namespace TTT.Game.Events.Player;

public class PlayerScreenMessageEvent(IPlayer player, string message,
  params object[] args) : PlayerMessageEvent(player, message, args) { }