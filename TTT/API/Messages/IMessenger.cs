using TTT.Api.Player;

namespace TTT.Api.Messages;

public interface IMessenger {
  /// <summary>
  ///   Attempt to send a message to a player.
  /// </summary>
  /// <param name="player">The player to send the message to.</param>
  /// <param name="message">The message to send</param>
  /// <typeparam name="T"></typeparam>
  Task<bool> Message(IPlayer player, string message);
}