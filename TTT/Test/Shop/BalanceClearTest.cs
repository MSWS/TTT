using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Roles;
using TTT.Karma;
using TTT.Shop.Listeners;
using Xunit;

namespace TTT.Test.Shop;

public class BalanceClearTest(IServiceProvider provider) {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [Fact]
  public async Task Balances_ShouldBeCleared_OnGameStart() {
    bus.RegisterListener(new RoundShopClearer(provider));
    var player = TestPlayer.Random();
    finder.AddPlayer(player);
    finder.AddPlayer(TestPlayer.Random());

    shop.AddBalance(player, 10);

    var game = games.CreateGame();
    game?.Start();
    game?.EndGame();

    var newBalance = await shop.Load(player);

    Assert.Equal(0, newBalance);
  }

  [Fact]
  public async Task RoleAssignCreditor_ShouldNotBeOverriden_OnGameStart() {
    bus.RegisterListener(new RoleAssignCreditor(provider));
    bus.RegisterListener(new RoundShopClearer(provider));
    var player = TestPlayer.Random();
    finder.AddPlayer(player);
    finder.AddPlayer(TestPlayer.Random());

    var karmaService = provider.GetService<IKarmaService>();
    if (karmaService != null) await karmaService.Write(player, 80);

    var game = games.CreateGame();
    game?.Start();

    await Task.Delay(TimeSpan.FromMilliseconds(10),
      TestContext.Current.CancellationToken);

    var newBalance = await shop.Load(player);

    var expected                                                    = 100;
    if (roles.GetRoles(player).Any(r => r is TraitorRole)) expected = 120;

    Assert.Equal(expected, newBalance);
  }
}