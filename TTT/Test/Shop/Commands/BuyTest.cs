using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Command;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Locale;
using TTT.Shop;
using TTT.Shop.Commands;
using TTT.Test.Game.Command;
using Xunit;

namespace TTT.Test.Shop.Commands;

public class BuyTest {
  private readonly IMsgLocalizer locale;
  private readonly ICommandManager manager;
  private readonly IServiceProvider provider;
  private readonly IShop shop;

  public BuyTest(IServiceProvider provider) {
    manager = provider.GetRequiredService<ICommandManager>();
    shop    = provider.GetRequiredService<IShop>();
    locale  = provider.GetRequiredService<IMsgLocalizer>();

    this.provider = provider;

    manager.RegisterCommand(new BuyCommand(provider));
  }

  private void startGame() {
    var games   = provider.GetRequiredService<IGameManager>();
    var players = provider.GetRequiredService<IPlayerFinder>();

    players.AddPlayers(TestPlayer.Random(), TestPlayer.Random());

    games.CreateGame()?.Start();
  }

  [Fact]
  public async Task BuyCommand_Exists() {
    var result =
      await manager.ProcessCommand(new TestCommandInfo(provider, null, "buy"));

    Assert.Equal(CommandResult.PLAYER_ONLY, result);
  }

  [Fact]
  public async Task Buy_WithNoArg_RequiresUsage() {
    startGame();
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "buy");
    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.PRINT_USAGE, result);
  }

  [Fact]
  public async Task Buy_NonExistentItem_Fails() {
    startGame();
    var player = TestPlayer.Random();
    var info = new TestCommandInfo(provider, player, "buy", "NonExistentItem");
    var result = await manager.ProcessCommand(info);
    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains(locale[ShopMsgs.SHOP_ITEM_NOT_FOUND("NonExistentItem")],
      player.Messages);
  }

  [Fact]
  public async Task Buy_WithWrongQuery_Fails() {
    startGame();
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "buy", "Sword");
    shop.RegisterItem(new TestShopItem());
    var result = await manager.ProcessCommand(info);
    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains(locale[ShopMsgs.SHOP_ITEM_NOT_FOUND("Sword")],
      player.Messages);
  }

  [Fact]
  public async Task Buy_TooExpensive_Fails() {
    startGame();
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "buy", TestShopItem.ID);

    var item = new TestShopItem();
    shop.RegisterItem(item);
    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains(locale[ShopMsgs.SHOP_INSUFFICIENT_BALANCE(item, 0)],
      player.Messages);
  }

  [Fact]
  public async Task Buy_WithSuccess_DecreasesBalance() {
    startGame();
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "buy", TestShopItem.ID);

    shop.RegisterItem(new TestShopItem());
    await shop.Write(player, 150);
    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.SUCCESS, result);
    var bal = await shop.Load(player);
    Assert.Equal(50, bal);
  }

  [Fact]
  public async Task Buy_WithSuccess_GivesItem() {
    startGame();
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "buy", TestShopItem.ID);

    shop.RegisterItem(new TestShopItem());
    await shop.Write(player, 150);
    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Contains(TestShopItem.ID,
      shop.GetOwnedItems(player).Select(s => s.Name));
  }

  [Fact]
  public async Task Buy_OutsideOfGame_Fails() {
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "buy", TestShopItem.ID);

    shop.RegisterItem(new TestShopItem());
    await shop.Write(player, 150);
    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Single(player.Messages);
    Assert.Contains("currently closed", player.Messages.First());
  }
}