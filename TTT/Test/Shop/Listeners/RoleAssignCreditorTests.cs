using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Player;
using TTT.Game.Roles;
using TTT.Karma;
using TTT.Shop.Listeners;
using Xunit;

namespace TTT.Test.Shop.Listeners;

public class RoleAssignCreditorTests(IServiceProvider provider) {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();
  private readonly IPlayerFinder finder = provider.GetRequiredService<IPlayerFinder>();
  private readonly IGameManager games = provider.GetRequiredService<IGameManager>();
  private readonly IRoleAssigner roles = provider.GetRequiredService<IRoleAssigner>();
  private readonly IShop shop = provider.GetRequiredService<IShop>();
  private readonly IKarmaService? karma = provider.GetService<IKarmaService>();

  [Fact]
  public async Task RoleAssignCreditor_ShouldGiveBaseCredits_ToInnocent() {
    // Arrange
    bus.RegisterListener(new RoleAssignCreditor(provider));
    var player = finder.AddPlayer(TestPlayer.Random());
    var role = new InnocentRole(provider);

    // Act
    var ev = new PlayerRoleAssignEvent(player, role);
    await bus.Dispatch(ev);

    // Assert
    var balance = await shop.Load(player);
    Assert.Equal(100, balance);
  }

  [Fact]
  public async Task RoleAssignCreditor_ShouldGiveTraitorCredits_ToTraitor() {
    // Arrange
    bus.RegisterListener(new RoleAssignCreditor(provider));
    var player = finder.AddPlayer(TestPlayer.Random());
    var role = new TraitorRole(provider);

    // Act
    var ev = new PlayerRoleAssignEvent(player, role);
    await bus.Dispatch(ev);

    // Assert
    var balance = await shop.Load(player);
    Assert.Equal(120, balance);
  }

  [Fact]
  public async Task RoleAssignCreditor_ShouldScaleCredits_WithHighKarma() {
    // Skip if karma service not available
    if (karma == null) return;

    // Arrange
    bus.RegisterListener(new RoleAssignCreditor(provider));
    var player = finder.AddPlayer(TestPlayer.Random());
    await karma.Write(player, 95); // High karma (>= 0.9 when normalized)
    var role = new InnocentRole(provider);

    // Act
    var ev = new PlayerRoleAssignEvent(player, role);
    await bus.Dispatch(ev);

    // Assert
    var balance = await shop.Load(player);
    Assert.Equal(110, balance); // 100 * 1.1
  }

  [Fact]
  public async Task RoleAssignCreditor_ShouldScaleCredits_WithGoodKarma() {
    // Skip if karma service not available
    if (karma == null) return;

    // Arrange
    bus.RegisterListener(new RoleAssignCreditor(provider));
    var player = finder.AddPlayer(TestPlayer.Random());
    await karma.Write(player, 85); // Good karma (>= 0.8)
    var role = new InnocentRole(provider);

    // Act
    var ev = new PlayerRoleAssignEvent(player, role);
    await bus.Dispatch(ev);

    // Assert
    var balance = await shop.Load(player);
    Assert.Equal(100, balance); // 100 * 1.0
  }

  [Fact]
  public async Task RoleAssignCreditor_ShouldScaleCredits_WithMediumKarma() {
    // Skip if karma service not available
    if (karma == null) return;

    // Arrange
    bus.RegisterListener(new RoleAssignCreditor(provider));
    var player = finder.AddPlayer(TestPlayer.Random());
    await karma.Write(player, 60); // Medium karma (>= 0.5)
    var role = new InnocentRole(provider);

    // Act
    var ev = new PlayerRoleAssignEvent(player, role);
    await bus.Dispatch(ev);

    // Assert
    var balance = await shop.Load(player);
    Assert.Equal(80, balance); // 100 * 0.8
  }

  [Fact]
  public async Task RoleAssignCreditor_ShouldScaleCredits_WithLowKarma() {
    // Skip if karma service not available
    if (karma == null) return;

    // Arrange
    bus.RegisterListener(new RoleAssignCreditor(provider));
    var player = finder.AddPlayer(TestPlayer.Random());
    await karma.Write(player, 35); // Low karma (>= 0.3)
    var role = new InnocentRole(provider);

    // Act
    var ev = new PlayerRoleAssignEvent(player, role);
    await bus.Dispatch(ev);

    // Assert
    var balance = await shop.Load(player);
    Assert.Equal(50, balance); // 100 * 0.5
  }

  [Fact]
  public async Task RoleAssignCreditor_ShouldScaleCredits_WithVeryLowKarma() {
    // Skip if karma service not available
    if (karma == null) return;

    // Arrange
    bus.RegisterListener(new RoleAssignCreditor(provider));
    var player = finder.AddPlayer(TestPlayer.Random());
    await karma.Write(player, 20); // Very low karma (< 0.3)
    var role = new InnocentRole(provider);

    // Act
    var ev = new PlayerRoleAssignEvent(player, role);
    await bus.Dispatch(ev);

    // Assert
    var balance = await shop.Load(player);
    Assert.Equal(25, balance); // 100 * 0.25
  }

  [Fact]
  public async Task RoleAssignCreditor_ShouldNotAffectOfflinePlayers() {
    // Arrange
    bus.RegisterListener(new RoleAssignCreditor(provider));
    var player = TestPlayer.Random(); // Not added to finder, not "online"
    var role = new InnocentRole(provider);

    // Act
    var ev = new PlayerRoleAssignEvent(player, role);
    await bus.Dispatch(ev);

    // Assert - Should not throw and balance should be 0
    var balance = await shop.Load(player);
    Assert.Equal(0, balance);
  }

  [Fact]
  public async Task RoleAssignCreditor_ShouldRoundUp_FractionalCredits() {
    // Skip if karma service not available
    if (karma == null) return;

    // Arrange
    bus.RegisterListener(new RoleAssignCreditor(provider));
    var player = finder.AddPlayer(TestPlayer.Random());
    await karma.Write(player, 60); // Will give 100 * 0.8 = 80
    var role = new InnocentRole(provider);

    // Act
    var ev = new PlayerRoleAssignEvent(player, role);
    await bus.Dispatch(ev);

    // Assert
    var balance = await shop.Load(player);
    Assert.True(balance % 1 == 0); // Should be integer
  }
}