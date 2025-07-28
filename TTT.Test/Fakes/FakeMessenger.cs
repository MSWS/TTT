using TTT.Api;
using TTT.Api.Events;
using TTT.Api.Messages;
using TTT.Api.Player;
using TTT.Game;
using TTT.Game.Events.Player;

namespace TTT.Test.Fakes;

public class FakeMessenger(IEventBus bus) : EventModifiedMessenger(bus) {
  override protected Task<bool> SendMessage(IPlayer player, string message) {
    if (player is not TestPlayer testPlayer)
      throw new ArgumentException("Player must be a TestPlayer",
        nameof(player));

    testPlayer.Messages.Add(message);
    return Task.FromResult(true);
  }
}