using TTT.Api;
using TTT.Api.Events;
using TTT.Api.Player;
using TTT.Game.Events.Player;

namespace TTT.Test.Fakes;

public class FakeMessenger(IEventBus bus) : IMessenger {
  public Task<bool> Message(IPlayer player, string message) {
    if (player is not TestPlayer testPlayer)
      throw new ArgumentException("Player must be a TestPlayer",
        nameof(player));

    var messageEvent = new PlayerMessageEvent(testPlayer, message);
    bus.Dispatch(messageEvent);
    if (messageEvent.IsCanceled) return Task.FromResult(false);
    testPlayer.Messages.Add(messageEvent.Message);
    return Task.FromResult(true);
  }
}