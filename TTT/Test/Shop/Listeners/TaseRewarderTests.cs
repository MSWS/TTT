using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;
using TTT.Shop.Listeners;
using Xunit;

namespace TTT.Test.Shop.Listeners;

public class TaseRewarderTests(IServiceProvider provider) {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();
  private readonly IPlayerFinder finder = provider.GetRequiredService<IPlayerFinder>();
  private readonly IGameManager games = provider.GetRequiredService<IGameManager>();
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [Fact]
  public async Task TaseRewarder_ShouldGiveCredits_OnSuccessfulTase() {
    // Arrange
    bus.RegisterListener(new TaseRewarder(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    var game = games.CreateGame();
    game?.Start();

    var initialBalance = await shop.Load(attacker);

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 95) {
      Weapon = "weapon_taser"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    var newBalance = await shop.Load(attacker);
    Assert.Equal(initialBalance + 30, newBalance);
  }

  [Fact]
  public async Task TaseRewarder_ShouldCancelDamage_OnTase() {
    // Arrange
    bus.RegisterListener(new TaseRewarder(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    var game = games.CreateGame();
    game?.Start();

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 95) {
      Weapon = "weapon_taser"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    Assert.True(dmgEvent.IsCanceled);
  }

  [Fact]
  public async Task TaseRewarder_ShouldNotReward_WhenGameNotInProgress() {
    // Arrange
    bus.RegisterListener(new TaseRewarder(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    // Don't start the game
    games.CreateGame();

    var initialBalance = await shop.Load(attacker);

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 95) {
      Weapon = "weapon_taser"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    var newBalance = await shop.Load(attacker);
    Assert.Equal(initialBalance, newBalance);
  }

  [Fact]
  public async Task TaseRewarder_ShouldNotReward_WhenWeaponIsNull() {
    // Arrange
    bus.RegisterListener(new TaseRewarder(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    var game = games.CreateGame();
    game?.Start();

    var initialBalance = await shop.Load(attacker);

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 95) {
      Weapon = null
    };
    bus.Dispatch(dmgEvent);

    // Assert
    var newBalance = await shop.Load(attacker);
    Assert.Equal(initialBalance, newBalance);
  }

  [Fact]
  public async Task TaseRewarder_ShouldNotReward_WhenWeaponIsNotTaser() {
    // Arrange
    bus.RegisterListener(new TaseRewarder(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    var game = games.CreateGame();
    game?.Start();

    var initialBalance = await shop.Load(attacker);

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 95) {
      Weapon = "weapon_ak47"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    var newBalance = await shop.Load(attacker);
    Assert.Equal(initialBalance, newBalance);
    Assert.False(dmgEvent.IsCanceled);
  }

  [Fact]
  public async Task TaseRewarder_ShouldNotReward_WhenAttackerIsNull() {
    // Arrange
    bus.RegisterListener(new TaseRewarder(provider));
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    var game = games.CreateGame();
    game?.Start();

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, null, 95) {
      Weapon = "weapon_taser"
    };
    bus.Dispatch(dmgEvent);

    // Assert - Should not throw and should cancel
    Assert.True(dmgEvent.IsCanceled);
  }

  [Theory]
  [InlineData("WEAPON_TASER")]
  [InlineData("Taser")]
  [InlineData("zeus")]
  public async Task TaseRewarder_ShouldReward_WithCaseInsensitiveTaserNames(string weaponName) {
    // Arrange
    bus.RegisterListener(new TaseRewarder(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    var game = games.CreateGame();
    game?.Start();

    var initialBalance = await shop.Load(attacker);

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 95) {
      Weapon = weaponName
    };
    bus.Dispatch(dmgEvent);

    // Assert
    var newBalance = await shop.Load(attacker);
    Assert.Equal(initialBalance + 30, newBalance);
  }
}