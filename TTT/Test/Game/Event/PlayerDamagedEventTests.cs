using TTT.Game.Events.Player;
using Xunit;

namespace TTT.Test.Game.Event;

public class PlayerDamagedEventTests {
  [Fact]
  public void PlayerDamagedEvent_ShouldCalculateDamage_FromHpDifference() {
    // Arrange
    var player = TestPlayer.Random();
    player.Health = 100;

    // Act
    var ev = new PlayerDamagedEvent(player, null, 75);

    // Assert
    Assert.Equal(25, ev.DmgDealt);
    Assert.Equal(75, ev.HpLeft);
  }

  [Fact]
  public void PlayerDamagedEvent_ShouldAllowSettingHpLeft() {
    // Arrange
    var player = TestPlayer.Random();
    player.Health = 100;
    var ev = new PlayerDamagedEvent(player, null, 80);

    // Act
    ev.HpLeft = 50;

    // Assert
    Assert.Equal(50, ev.HpLeft);
    Assert.Equal(50, ev.DmgDealt);
  }

  [Fact]
  public void PlayerDamagedEvent_ShouldAllowZeroHealth() {
    // Arrange
    var player = TestPlayer.Random();
    player.Health = 100;

    // Act
    var ev = new PlayerDamagedEvent(player, null, 0);

    // Assert
    Assert.Equal(0, ev.HpLeft);
    Assert.Equal(100, ev.DmgDealt);
  }

  [Fact]
  public void PlayerDamagedEvent_ShouldAllowNegativeHealth() {
    // Arrange
    var player = TestPlayer.Random();
    player.Health = 100;

    // Act
    var ev = new PlayerDamagedEvent(player, null, -10);

    // Assert
    Assert.Equal(-10, ev.HpLeft);
    Assert.Equal(110, ev.DmgDealt);
  }

  [Fact]
  public void PlayerDamagedEvent_ShouldHandleHealing() {
    // Arrange
    var player = TestPlayer.Random();
    player.Health = 50;

    // Act - HpLeft > current health means healing
    var ev = new PlayerDamagedEvent(player, null, 75);

    // Assert
    Assert.Equal(75, ev.HpLeft);
    Assert.Equal(-25, ev.DmgDealt); // Negative damage = healing
  }

  [Fact]
  public void PlayerDamagedEvent_ShouldIncludeAttacker() {
    // Arrange
    var player = TestPlayer.Random();
    var attacker = TestPlayer.Random();

    // Act
    var ev = new PlayerDamagedEvent(player, attacker, 75);

    // Assert
    Assert.Equal(attacker, ev.Attacker);
  }

  [Fact]
  public void PlayerDamagedEvent_ShouldAllowNullAttacker() {
    // Arrange
    var player = TestPlayer.Random();

    // Act
    var ev = new PlayerDamagedEvent(player, null, 75);

    // Assert
    Assert.Null(ev.Attacker);
  }

  [Fact]
  public void PlayerDamagedEvent_ShouldSupportCancellation() {
    // Arrange
    var player = TestPlayer.Random();
    var ev = new PlayerDamagedEvent(player, null, 75);

    // Act
    ev.IsCanceled = true;

    // Assert
    Assert.True(ev.IsCanceled);
  }

  [Fact]
  public void PlayerDamagedEvent_ShouldIncludeWeaponInfo() {
    // Arrange
    var player = TestPlayer.Random();
    var ev = new PlayerDamagedEvent(player, null, 75) {
      Weapon = "weapon_ak47"
    };

    // Assert
    Assert.Equal("weapon_ak47", ev.Weapon);
  }

  [Fact]
  public void PlayerDamagedEvent_ShouldIncludeArmorInfo() {
    // Arrange
    var player = TestPlayer.Random();
    var ev = new PlayerDamagedEvent(player, null, 75) {
      ArmorDamage = 10,
      ArmorRemaining = 40
    };

    // Assert
    Assert.Equal(10, ev.ArmorDamage);
    Assert.Equal(40, ev.ArmorRemaining);
  }

  [Theory]
  [InlineData(100, 75, 25)]
  [InlineData(100, 0, 100)]
  [InlineData(50, 25, 25)]
  [InlineData(100, 100, 0)]
  [InlineData(1, 0, 1)]
  public void PlayerDamagedEvent_ShouldCalculateDamage_Correctly(int initialHp, int hpLeft, int expectedDamage) {
    // Arrange
    var player = TestPlayer.Random();
    player.Health = initialHp;

    // Act
    var ev = new PlayerDamagedEvent(player, null, hpLeft);

    // Assert
    Assert.Equal(expectedDamage, ev.DmgDealt);
  }

  [Fact]
  public void PlayerDamagedEvent_ShouldRecalculateDamage_WhenHpLeftChanges() {
    // Arrange
    var player = TestPlayer.Random();
    player.Health = 100;
    var ev = new PlayerDamagedEvent(player, null, 75);
    Assert.Equal(25, ev.DmgDealt);

    // Act
    ev.HpLeft = 60;

    // Assert
    Assert.Equal(40, ev.DmgDealt);
  }
}