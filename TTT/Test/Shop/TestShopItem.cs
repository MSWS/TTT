using TTT.API.Player;
using TTT.Shop;

namespace TTT.Test.Shop;

public class TestShopItem : IShopItem {
  public void Dispose() { }
  public string Name => "Test Item";
  public string Id => ID;
  public const string ID = "ttt.test.item.testitem";
  public string Description => "A test item for unit tests.";
  public ShopItemConfig Config { get; } = new TestItemConfig();

  public void OnPurchase(IOnlinePlayer player) { }

  public PurchaseResult CanPurchase(IOnlinePlayer player) {
    return PurchaseResult.SUCCESS;
  }

  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }
}

public record TestItemConfig(int Price = 100) : ShopItemConfig { }