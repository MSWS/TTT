using JetBrains.Annotations;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Game.Events.Player;
using Xunit;

namespace TTT.Test.Messages;

public class JoinMessageTest(IEventBus bus, IMessenger msg,
  IPlayerFinder finder) {
  [Fact]
  public void TestJoinMessage() {
    // Arrange
    var listener = new JoinMessageListener(bus, msg);
    var player   = TestPlayer.Random();
    bus.RegisterListener(listener);

    // Act
    finder.AddPlayer(player);

    // Assert
    Assert.Single(player.Messages);
    Assert.Equal("Hello, World!", player.Messages[0]);
  }

  private class JoinMessageListener(IEventBus bus, IMessenger msg) : IListener {
    public void Dispose() { bus.UnregisterListener(this); }

    [EventHandler]
    [UsedImplicitly]
    public void OnJoin(PlayerJoinEvent ev) {
      msg.Message(ev.Player, "Hello, World!");
    }
  }
}