using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game;

namespace TTT.CS2;

public class CS2Messenger(IServiceProvider provider)
  : EventModifiedMessenger(provider.GetRequiredService<IEventBus>()) {
  private readonly ILogger logger = provider
   .GetRequiredService<ILoggerFactory>()
   .CreateLogger("TTT - CS2");

  private CCSPlayerController? getPlayer(IPlayer player) {
    if (!ulong.TryParse(player.Id, out var steamId)) return null;
    var gamePlayer = Utilities.GetPlayerFromSteamId(steamId);
    return gamePlayer is not { IsValid: true } ? null : gamePlayer;
  }

  override protected async Task<bool> SendMessage(IPlayer? player,
    string message) {
    if (player == null) {
      Console.WriteLine(message);
      logger.LogInformation(message);
      await Server.NextFrameAsync(() => {
        // TODO: Looks like this is broken due to the recent CS2 update
        Console.WriteLine(message);
        logger.LogInformation(message);
      });
      return true;
    }

    var success = false;
    return await Server.NextFrameAsync(() => {
        var gamePlayer = getPlayer(player);
        gamePlayer?.PrintToChat(message);
        success = gamePlayer != null;
      })
     .ContinueWith(_ => success);
  }

  public override async Task<bool> BackgroundMsg(IPlayer? player,
    string message) {
    if (player == null) return await Message(null, message);

    var success = false;
    return await Server.NextFrameAsync(() => {
        var gamePlayer = getPlayer(player);
        gamePlayer?.PrintToConsole(message);
        success = gamePlayer != null;
      })
     .ContinueWith(_ => success);
  }

  public override async Task<bool> ScreenMsg(IPlayer? player, string message) {
    if (player == null) return await Message(null, message);
    var success = false;
    return await Server.NextFrameAsync(() => {
        var gamePlayer = getPlayer(player);
        gamePlayer?.PrintToCenter(message);
        success = gamePlayer != null;
      })
     .ContinueWith(_ => success);
  }
}