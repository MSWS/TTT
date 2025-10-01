using TTT.API.Player;
using TTT.Shop;

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