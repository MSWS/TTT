using TTT.API.Events;
using TTT.API.Player;
using TTT.Game;

namespace TTT.Test.Fakes;

public class TestMessenger(IEventBus bus) : EventModifiedMessenger(bus) {
  override protected Task<bool> SendMessage(IPlayer? player, string message) {
    if (player is not TestPlayer testPlayer)
      throw new ArgumentException("Player must be a TestPlayer",
        nameof(player));

    testPlayer.Messages.Add(message);
    return Task.FromResult(true);
  }
}