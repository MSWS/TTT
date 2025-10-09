using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.Items.OneHitKnife;
using TTT.Game.Events.Player;
using TTT.Game.Roles;
using Xunit;

namespace TTT.Test.Shop.Items;

public class OneHitKnifeTests(IServiceProvider provider) {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();
  private readonly IPlayerFinder finder = provider.GetRequiredService<IPlayerFinder>();
  private readonly IGameManager games = provider.GetRequiredService<IGameManager>();
  private readonly IRoleAssigner roles = provider.GetRequiredService<IRoleAssigner>();
  private readonly IShop shop = provider.GetRequiredService<IShop>();
  private readonly OneHitKnife item = new(provider);

  [Fact]
  public void OneHitKnife_ShouldKill_OnKnifeHit() {
    // Arrange
    bus.RegisterListener(new OneHitKnifeListener(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    roles.Write(attacker, [new TraitorRole(provider)]);
    roles.Write(victim, [new InnocentRole(provider)]);
    
    var game = games.CreateGame();
    game?.Start();

    shop.GiveItem(attacker, item);

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 99) {
      Weapon = "weapon_knife"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    Assert.Equal(0, dmgEvent.HpLeft);
  }

  [Fact]
  public void OneHitKnife_ShouldRemoveItem_AfterKill() {
    // Arrange
    bus.RegisterListener(new OneHitKnifeListener(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    roles.Write(attacker, [new TraitorRole(provider)]);
    roles.Write(victim, [new InnocentRole(provider)]);
    
    var game = games.CreateGame();
    game?.Start();

    shop.GiveItem(attacker, item);
    Assert.True(shop.HasItem<OneHitKnife>(attacker));

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 99) {
      Weapon = "weapon_knife"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    Assert.False(shop.HasItem<OneHitKnife>(attacker));
  }

  [Fact]
  public void OneHitKnife_ShouldNotKill_WhenNotOwned() {
    // Arrange
    bus.RegisterListener(new OneHitKnifeListener(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    roles.Write(attacker, [new TraitorRole(provider)]);
    roles.Write(victim, [new InnocentRole(provider)]);
    
    var game = games.CreateGame();
    game?.Start();

    // Don't give the item
    Assert.False(shop.HasItem<OneHitKnife>(attacker));

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 90) {
      Weapon = "weapon_knife"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    Assert.NotEqual(0, dmgEvent.HpLeft);
  }

  [Fact]
  public void OneHitKnife_ShouldNotKill_WhenWeaponIsNotKnife() {
    // Arrange
    bus.RegisterListener(new OneHitKnifeListener(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    roles.Write(attacker, [new TraitorRole(provider)]);
    roles.Write(victim, [new InnocentRole(provider)]);
    
    var game = games.CreateGame();
    game?.Start();

    shop.GiveItem(attacker, item);

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 90) {
      Weapon = "weapon_ak47"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    Assert.NotEqual(0, dmgEvent.HpLeft);
    Assert.True(shop.HasItem<OneHitKnife>(attacker)); // Item still owned
  }

  [Fact]
  public void OneHitKnife_ShouldNotKill_FriendlyFire_WhenDisabled() {
    // Arrange
    bus.RegisterListener(new OneHitKnifeListener(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    // Both traitors
    roles.Write(attacker, [new TraitorRole(provider)]);
    roles.Write(victim, [new TraitorRole(provider)]);
    
    var game = games.CreateGame();
    game?.Start();

    shop.GiveItem(attacker, item);

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 90) {
      Weapon = "weapon_knife"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    Assert.NotEqual(0, dmgEvent.HpLeft); // Should not kill
    Assert.True(shop.HasItem<OneHitKnife>(attacker)); // Item still owned
  }

  [Fact]
  public void OneHitKnife_ShouldNotTrigger_WhenGameNotInProgress() {
    // Arrange
    bus.RegisterListener(new OneHitKnifeListener(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    roles.Write(attacker, [new TraitorRole(provider)]);
    roles.Write(victim, [new InnocentRole(provider)]);
    
    // Don't start the game
    games.CreateGame();

    shop.GiveItem(attacker, item);

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 90) {
      Weapon = "weapon_knife"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    Assert.NotEqual(0, dmgEvent.HpLeft);
  }

  [Fact]
  public void OneHitKnife_ShouldNotTrigger_WhenAttackerIsNull() {
    // Arrange
    bus.RegisterListener(new OneHitKnifeListener(provider));
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    var game = games.CreateGame();
    game?.Start();

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, null, 90) {
      Weapon = "weapon_knife"
    };
    bus.Dispatch(dmgEvent);

    // Assert
    Assert.NotEqual(0, dmgEvent.HpLeft);
  }

  [Theory]
  [InlineData("weapon_knife")]
  [InlineData("weapon_knife_t")]
  [InlineData("weapon_bayonet")]
  public void OneHitKnife_ShouldKill_WithVariousKnifeWeapons(string knifeWeapon) {
    // Arrange
    bus.RegisterListener(new OneHitKnifeListener(provider));
    var attacker = finder.AddPlayer(TestPlayer.Random());
    var victim = finder.AddPlayer(TestPlayer.Random());
    
    roles.Write(attacker, [new TraitorRole(provider)]);
    roles.Write(victim, [new InnocentRole(provider)]);
    
    var game = games.CreateGame();
    game?.Start();

    shop.GiveItem(attacker, item);

    // Act
    var dmgEvent = new PlayerDamagedEvent(victim, attacker, 99) {
      Weapon = knifeWeapon
    };
    bus.Dispatch(dmgEvent);

    // Assert
    Assert.Equal(0, dmgEvent.HpLeft);
  }
}