using CounterStrikeSharp.API.Core;
using TTT.Api.Events;

namespace TTT.CS2.Messages;

public class ChatMessenger(IEventBus bus) : GameMessenger(bus) {
  override protected Task<bool> SendMessage(CCSPlayerController gamePlayer,
    string message) {
    gamePlayer.PrintToChat(message);
    return Task.FromResult(true);
  }
}