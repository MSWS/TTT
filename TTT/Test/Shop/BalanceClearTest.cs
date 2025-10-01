using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Shop.Listeners;
using Xunit;

namespace TTT.Test.Shop;

public class BalanceClearTest(IServiceProvider provider) {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

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

    var newBalance = await shop.Load(player);

    Assert.Equal(0, newBalance);
  }
}