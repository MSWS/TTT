using TTT.API.Player;

namespace TTT.API.Messages;

public interface IOnlineMessenger : IMessenger {
  Task<bool> IMessenger.Message(IPlayer player, string message) {
    if (player is not IOnlinePlayer onlinePlayer)
      return
        Task.FromResult(false); // Cannot send message to non-online players

    return Message(onlinePlayer, message);
  }

  Task<bool> Message(IOnlinePlayer player, string message);

  async Task<bool> MessageAll(IPlayerFinder finder, string message) {
    var tasks = finder.GetOnline()
     .Select(onlinePlayer => Message(onlinePlayer, message))
     .ToList();

    var results = await Task.WhenAll(tasks);
    return results.All(r => r);
  }

  async Task<bool> BackgroundMsgAll(IPlayerFinder finder, string message) {
    var tasks = finder.GetOnline()
     .Select(onlinePlayer => BackgroundMsg(onlinePlayer, message))
     .ToList();

    var results = await Task.WhenAll(tasks);
    return results.All(r => r);
  }

  async Task<bool> ScreenMsgAll(IPlayerFinder finder, string message) {
    var tasks = finder.GetOnline()
     .Select(onlinePlayer => ScreenMsg(onlinePlayer, message))
     .ToList();

    var results = await Task.WhenAll(tasks);
    return results.All(r => r);
  }

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