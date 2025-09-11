using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.Shop;
using TTT.Shop.Commands;
using TTT.Test.Game.Command;
using Xunit;

namespace TTT.Test.Shop.Commands;

public class BuyTest {
  private readonly ICommandManager manager;
  private readonly IServiceProvider provider;
  private readonly IShop shop;

  public BuyTest(IServiceProvider provider) {
    manager       = provider.GetRequiredService<ICommandManager>();
    shop          = provider.GetRequiredService<IShop>();
    this.provider = provider;

    manager.RegisterCommand(new BuyCommand(provider));
  }

  [Fact]
  public async Task BuyCommand_Exists() {
    var result =
      await manager.ProcessCommand(new TestCommandInfo(provider, null, "buy"));

    Assert.Equal(CommandResult.PLAYER_ONLY, result);
  }

  [Fact]
  public async Task Buy_WithNoArg_RequiresUsage() {
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "buy");
    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.INVALID_ARGS, result);
    Assert.Contains("Please specify an item to buy.", player.Messages);
  }

  [Fact]
  public async Task Buy_NonExistentItem_Fails() {
    var player = TestPlayer.Random();
    var info = new TestCommandInfo(provider, player, "buy", "NonExistentItem");
    var result = await manager.ProcessCommand(info);
    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains("Item 'NonExistentItem' not found.", player.Messages);
  }

  [Fact]
  public async Task Buy_WithWrongQuery_Fails() {
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "buy", "Sword");
    var result = await manager.ProcessCommand(info);

    shop.RegisterItem(new TestShopItem());

    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains("Item 'Sword' not found.", player.Messages);
  }

  [Fact]
  public async Task Buy_TooExpensive_Fails() {
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "buy", TestShopItem.ID);

    shop.RegisterItem(new TestShopItem());
    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.ERROR, result);
    Assert.Contains(
      "You cannot afford 'Test Item'. It costs 100, but you have 0.",
      player.Messages);
  }

  [Fact]
  public async Task Buy_WithSuccess_DecreasesBalance() {
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
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "buy", TestShopItem.ID);

    shop.RegisterItem(new TestShopItem());
    await shop.Write(player, 150);
    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Contains(TestShopItem.ID,
      shop.GetOwnedItems(player).Select(s => s.Id));
  }
}