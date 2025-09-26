using TTT.Game.Events.Player;
using Xunit;

namespace TTT.Test.Game.Event;

public class DamageEventTest {
  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void HpLeft_Set_FailsIfInvalid(int hp) {
    var ev = new PlayerDamagedEvent(TestPlayer.Random(), null, 0, 50);
    Assert.ThrowsAny<ArgumentException>(() => ev.HpLeft = hp);
  }

  [Fact]
  public void HpLeft_Set_DoesNothingIfSame() {
    var ev = new PlayerDamagedEvent(TestPlayer.Random(), null, 0, 0);
    ev.HpLeft = 0;
  }

  [Fact]
  public void HpLeft_Set_MarksDirty() {
    var ev = new PlayerDamagedEvent(TestPlayer.Random(), null, 0, 50);
    ev.HpLeft = 49;
    Assert.True(ev.HpModified);
  }

  [Fact]
  public void HpLeft_SetViaBody_MarksDirty() {
    var ev =
      new PlayerDamagedEvent(TestPlayer.Random(), null, 0, 50) { HpLeft = 49 };
    Assert.True(ev.HpModified);
  }

  [Fact]
  public void HpLeft_SetWithSame_DoesNotMarkDirty() {
    var ev = new PlayerDamagedEvent(TestPlayer.Random(), null, 0, 50);
    ev.HpLeft = 50;
    Assert.False(ev.HpModified);
  }
}