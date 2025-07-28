using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.Api.Events;
using TTT.Api.Messages;
using TTT.Api.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.Messages;

public abstract class GameMessenger(IEventBus bus) : IOnlineMessenger {
  public Task<bool> Message(IOnlinePlayer player, string message) {
    if (!ulong.TryParse(player.Id, out var steamId))
      return Task.FromResult(false);

    var gamePlayer = Utilities.GetPlayerFromSteamId(steamId);
    if (gamePlayer == null || !gamePlayer.IsValid || gamePlayer.IsBot)
      return Task.FromResult(false);

    var messageEvent = new PlayerMessageEvent(player, message);
    bus.Dispatch(messageEvent);
    if (messageEvent.IsCanceled) return Task.FromResult(false);

    return SendMessage(gamePlayer, messageEvent.Message);
  }

  abstract protected Task<bool> SendMessage(CCSPlayerController gamePlayer,
    string message);
}