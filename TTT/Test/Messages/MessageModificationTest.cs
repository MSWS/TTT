using TTT.API.Events;
using TTT.API.Messages;
using TTT.Game.Events.Player;
using Xunit;

namespace TTT.Test.Messages;

public partial class MessageModificationTest(IEventBus bus,
  IMessenger messenger) {
  [Fact]
  public void Message_IsModified() {
    // Arrange
    var listener = new SimpleMessageSubstitution<PlayerMessageEvent>(bus);
    bus.RegisterListener(listener);
    var player = TestPlayer.Random();

    // Act
    messenger.Message(player, ORIGINAL_MESSAGE);

    // Assert
    Assert.Single(player.Messages);
    Assert.Equal(MODIFIED_MESSAGE, player.Messages[0]);
  }

  [Fact]
  public void Message_Args_AreModified() {
    var listener = new SimpleArgsSubstitution<PlayerMessageEvent>(bus);
    bus.RegisterListener(listener);
    var player = TestPlayer.Random();

    messenger.Message(player, ORIGINAL_MESSAGE + " {0}", "Original Arg");

    Assert.Single(player.Messages);
    Assert.Equal(ORIGINAL_MESSAGE + " Modified Arg", player.Messages[0]);
  }

  [Fact]
  public void BackgroundMessage_IsModified() {
    // Arrange
    var listener =
      new SimpleMessageSubstitution<PlayerBackgroundMessageEvent>(bus);
    bus.RegisterListener(listener);
    var player = TestPlayer.Random();

    // Act
    messenger.BackgroundMsg(player, ORIGINAL_MESSAGE);

    // Assert
    Assert.Single(player.Messages);
    Assert.Equal(MODIFIED_MESSAGE, player.Messages[0]);
  }

  [Fact]
  public void BackgroundMessage_Args_AreModified() {
    var listener =
      new SimpleArgsSubstitution<PlayerBackgroundMessageEvent>(bus);
    bus.RegisterListener(listener);
    var player = TestPlayer.Random();

    messenger.BackgroundMsg(player, ORIGINAL_MESSAGE + " {0}", "Original Arg");
    Assert.Single(player.Messages);
    Assert.Equal(ORIGINAL_MESSAGE + " Modified Arg", player.Messages[0]);
  }

  [Fact]
  public void ScreenMessage_IsModified() {
    // Arrange
    var listener = new SimpleMessageSubstitution<PlayerScreenMessageEvent>(bus);
    bus.RegisterListener(listener);
    var player = TestPlayer.Random();

    // Act
    messenger.ScreenMsg(player, ORIGINAL_MESSAGE);

    // Assert
    Assert.Single(player.Messages);
    Assert.Equal(MODIFIED_MESSAGE, player.Messages[0]);
  }

  [Fact]
  public void ScreenMessage_Args_AreModified() {
    var listener = new SimpleArgsSubstitution<PlayerScreenMessageEvent>(bus);
    bus.RegisterListener(listener);
    var player = TestPlayer.Random();

    messenger.ScreenMsg(player, ORIGINAL_MESSAGE + " {0}", "Original Arg");

    Assert.Single(player.Messages);
    Assert.Equal(ORIGINAL_MESSAGE + " Modified Arg", player.Messages[0]);
  }
}