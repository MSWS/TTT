using System.ComponentModel.Design;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Shop;
using Xunit;

namespace TTT.Test.Shop;

public class BalanceClearTest(IServiceProvider provider) {
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  [Fact]
  public async Task Balances_ShouldBeCleared_OnGameStart() {
    bus.RegisterListener(new RoundBalanceClearer(provider));
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