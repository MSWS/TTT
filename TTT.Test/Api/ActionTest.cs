using TTT.Api;
using TTT.Test.Fakes;
using Xunit;

namespace TTT.Test.Api;

public class ActionTest {
  [Fact]
  public void Format_ShouldFormat_ActionWithOther() {
    // Arrange
    var alice = new TestPlayer("test-alice", "Alice");
    var bob   = new TestPlayer("test-bob", "Bob");

    // Act
    var result = new FakeAction(alice, bob, "action-id", "IDs", "unitly");

    // Assert
    Assert.Equal($"{alice} IDs {bob} unitly", (result as IAction).Format());
  }

  [Fact]
  public void Format_ShouldFormat_ActionWithoutOther() {
    // Arrange
    var alice = new TestPlayer("test-alice", "Alice");

    // Act
    var result = new FakeAction(alice, null, "action-thinks", "thinks",
      "deeply");

    // Assert
    Assert.Equal($"{alice} thinks deeply", (result as IAction).Format());
  }
}