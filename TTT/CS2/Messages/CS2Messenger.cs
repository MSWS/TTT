using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game;

namespace TTT.CS2.Messages;

public class CS2Messenger(IEventBus bus) : EventModifiedMessenger(bus) {
  private CCSPlayerController? getPlayer(IPlayer player) {
    if (!ulong.TryParse(player.Id, out var steamId)) return null;
    var gamePlayer = Utilities.GetPlayerFromSteamId(steamId);
    return gamePlayer is not { IsValid: true } ? null : gamePlayer;
  }

  override protected Task<bool> SendMessage(IPlayer player, string message) {
    var gamePlayer = getPlayer(player);
    getPlayer(player)?.PrintToChat(message);
    return Task.FromResult(gamePlayer != null);
  }

  public override Task<bool> BackgroundMsg(IPlayer player, string message) {
    var gamePlayer = getPlayer(player);
    gamePlayer?.PrintToConsole(message);
    return Task.FromResult(gamePlayer != null);
  }

  public override Task<bool> ScreenMsg(IPlayer player, string message) {
    var gamePlayer = getPlayer(player);
    gamePlayer?.PrintToCenter(message);
    return Task.FromResult(gamePlayer != null);
  }
}