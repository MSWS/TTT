using TTT.API.Player;

namespace TTT.API.Messages;

public interface IMessenger {
  Task<bool> Message(IPlayer? player, string message, params object[] args);

  void Debug(string msg, params object[] args);
  void DebugInform(string msg, params object[] args) { Debug(msg, args); }

  void DebugAnnounce(string msg, params object[] args) { Debug(msg, args); }

  Task<bool> MessageAll(string message, params object[] args);
  Task<bool> BackgroundMsgAll(string message, params object[] args);
  Task<bool> ScreenMsgAll(string message, params object[] args);

  /// <summary>
  ///   Attempt to send a message to a player without showing it on the screen.
  ///   This could mean sending to console, background file, or just
  ///   falling back to showing it on the screen.
  /// </summary>
  /// <param name="player"></param>
  /// <param name="message"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  Task<bool> BackgroundMsg(IPlayer? player, string message,
    params object[] args) {
    return Message(player, message, args);
  }

  /// <summary>
  ///   Attempt to send a message to a player that will be shown on the screen using
  ///   an alternative method, such as a popup or a notification.
  ///   May just fall back to showing it on the screen if no alternative is available.
  /// </summary>
  /// <param name="player"></param>
  /// <param name="message"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  Task<bool> ScreenMsg(IPlayer? player, string message, params object[] args) {
    return Message(player, message, args);
  }
}