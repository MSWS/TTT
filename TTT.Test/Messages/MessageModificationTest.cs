using TTT.Api.Events;
using TTT.Api.Messages;
using TTT.Game.Events.Player;
using Xunit;

namespace TTT.Test.Messages;

public class MessageModificationTest(IEventBus bus, IMessenger messenger) {
  private const string ORIGINAL_MESSAGE = "Original Message";
  private const string MODIFIED_MESSAGE = "Modified Message";

  private class MessageModifyListener(IEventBus bus) : IListener {
    public void Dispose() => bus.UnregisterListener(this);

    [EventHandler]
    public void OnMessage(PlayerMessageEvent ev) {
      ev.Message = MODIFIED_MESSAGE;
    }
  }

  [Fact]
  public void TestMessageModification() {
    // Arrange
    var listener = new MessageModifyListener(bus);
    bus.RegisterListener(listener);
    var player = TestPlayer.Random();

    // Act
    messenger.Message(player, ORIGINAL_MESSAGE);

    // Assert
    Assert.Single(player.Messages);
    Assert.Equal(MODIFIED_MESSAGE, player.Messages[0]);
  }
}