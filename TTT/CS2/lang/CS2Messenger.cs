using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Game;

namespace TTT.CS2.lang;

public class CS2Messenger(IServiceProvider provider)
  : EventModifiedMessenger(provider) {
  private CCSPlayerController? getPlayer(IPlayer player) {
    if (!ulong.TryParse(player.Id, out var steamId)) return null;
    var gamePlayer = Utilities.GetPlayerFromSteamId(steamId);
    return gamePlayer is not { IsValid: true } ? null : gamePlayer;
  }

  override protected async Task<bool> SendMessage(IPlayer? player,
    string message, params object[] args) {
    if (args.Length > 0) message = string.Format(message, args);
    if (player == null) {
      Console.WriteLine(message);
      return true;
    }

    var success = false;
    return await Server.NextWorldUpdateAsync(() => {
        var gamePlayer = getPlayer(player);
        gamePlayer?.PrintToChat(message);
        success = gamePlayer != null;
      })
     .ContinueWith(_ => success);
  }

  public override void Debug(string msg, params object[] args) {
#if DEBUG
    _ = ((IMessenger)this).BackgroundMsgAll(
      $"[DEBUG] {string.Format(msg, args)}");
#endif
  }

  public override void DebugAnnounce(string msg, params object[] args) {
#if DEBUG
    _ = ((IMessenger)this).MessageAll(
      $"[DEBUG ANNOUNCE] {string.Format(msg, args)}");
#endif
  }

  public override void DebugInform(string msg, params object[] args) {
#if DEBUG
    _ = ((IMessenger)this).ScreenMsgAll(
      $"[DEBUG INFORM] {string.Format(msg, args)}");
#endif
  }

  public override async Task<bool> BackgroundMsg(IPlayer? player,
    string message, params object[] args) {
    if (args.Length > 0) message = string.Format(message, args);

    if (player == null) return await Message(null, message);

    var success = false;
    return await Server.NextWorldUpdateAsync(() => {
        var gamePlayer = getPlayer(player);
        gamePlayer?.PrintToConsole(message);
        success = gamePlayer != null;
      })
     .ContinueWith(_ => success);
  }

  public override async Task<bool> ScreenMsg(IPlayer? player, string message,
    params object[] args) {
    if (args.Length > 0) message = string.Format(message, args);
    if (player == null) return await Message(null, message);
    var success = false;
    return await Server.NextWorldUpdateAsync(() => {
        var gamePlayer = getPlayer(player);
        gamePlayer?.PrintToCenter(message);
        success = gamePlayer != null;
      })
     .ContinueWith(_ => success);
  }
}