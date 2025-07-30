using TTT.API.Player;

namespace TTT.API.Messages;

public interface IMessenger {
  /// <summary>
  ///   Attempt to send a message to a player.
  /// </summary>
  /// <param name="player">The player to send the message to.</param>
  /// <param name="message">The message to send</param>
  Task<bool> Message(IPlayer player, string message);
}