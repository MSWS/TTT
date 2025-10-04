using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Player;

namespace TTT.Test.Shop;

public class TestShopItem : IShopItem {
  public const string ID = "ttt.test.item.testitem";
  public void Dispose() { }
  public string Id => "Test Item";
  public string Name => ID;
  public string Description => "A test item for unit tests.";
  public ShopItemConfig Config { get; } = new TestItemConfig();

  public void OnPurchase(IOnlinePlayer player) { }

  public PurchaseResult CanPurchase(IOnlinePlayer player) {
    return PurchaseResult.SUCCESS;
  }

  public void Start() { }
}

public record TestItemConfig(int Price = 100) : ShopItemConfig { }