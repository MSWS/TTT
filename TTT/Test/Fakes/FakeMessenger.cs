using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Game;

namespace TTT.Test.Fakes;

public class FakeMessenger(IEventBus bus)
  : EventModifiedMessenger(bus), IOnlineMessenger {
  public Task<bool> Message(IOnlinePlayer player, string message) {
    return Message(player as IPlayer, message);
  }

  override protected Task<bool> SendMessage(IPlayer player, string message) {
    if (player is not TestPlayer testPlayer)
      throw new ArgumentException("Player must be a TestPlayer",
        nameof(player));

    testPlayer.Messages.Add(message);
    return Task.FromResult(true);
  }
}