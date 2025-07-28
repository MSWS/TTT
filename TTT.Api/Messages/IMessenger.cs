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

  /// <summary>
  ///   Attempt to send a message to a player without showing it on the screen.
  ///   This could mean sending to console, background file, or just
  ///   falling back to showing it on the screen.
  /// </summary>
  /// <param name="player"></param>
  /// <param name="message"></param>
  /// <returns></returns>
  Task<bool> BackgroundMsg(IPlayer player, string message) {
    return Message(player, message);
  }

  /// <summary>
  ///   Attempt to send a message to a player that will be shown on the screen using
  ///   an alternative method, such as a popup or a notification.
  ///   May just fall back to showing it on the screen if no alternative is available.
  /// </summary>
  /// <param name="player"></param>
  /// <param name="message"></param>
  /// <returns></returns>
  Task<bool> ScreenMsg(IPlayer player, string message) {
    return Message(player, message);
  }
}