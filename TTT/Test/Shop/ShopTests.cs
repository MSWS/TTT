using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Shop;
using TTT.Shop.Listeners;
using Xunit;

namespace TTT.Test.Shop;

public class ShopTests(IServiceProvider provider) {
  private readonly IShop shop = provider.GetRequiredService<IShop>();
  private readonly IOnlinePlayer player = TestPlayer.Random();
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  [Fact]
  public void GiveItem_ShowsInInventory() {
    shop.GiveItem(player, new TestShopItem());
    Assert.Single(shop.GetOwnedItems(player));
    Assert.Equal(TestShopItem.ID, shop.GetOwnedItems(player)[0].Name);
  }

  [Fact]
  public async Task ClearBalances_ClearsBalances() {
    shop.AddBalance(player, 500, "Test");
    Assert.Equal(500, await shop.Load(player));
    shop.ClearBalances();
    Assert.Equal(0, await shop.Load(player));
  }

  [Fact]
  public void ClearItems_ClearsItems() {
    shop.GiveItem(player, new TestShopItem());
    shop.ClearItems();
    Assert.Empty(shop.GetOwnedItems(player));
  }

  [Fact]
  public void Shop_ClearsItems_OnNewRound() {
    bus.RegisterListener(new RoundShopClearer(provider));

    finder.AddPlayers(player, TestPlayer.Random());
    games.CreateGame()?.Start();
    shop.GiveItem(player, new TestShopItem());
    games.ActiveGame?.EndGame();
    games.CreateGame()?.Start();

    Assert.Empty(shop.GetOwnedItems(player));
  }
}