using TTT.API.Player;
using TTT.Locale;

namespace TTT.Shop;

public class OneShotDeagle(IMsgLocalizer locale) : IShopItem {
  public string Name => locale[ShopMsgs.SHOP_ITEM_DEAGLE];
  public string Description => locale[ShopMsgs.SHOP_ITEM_DEAGLE_DESC];
  public int Price { get; }

  public void OnPurchase(IOnlinePlayer player) {
    throw new NotImplementedException();
  }

  public bool CanPurchase(IOnlinePlayer player) {
    throw new NotImplementedException();
  }
}

public record OneShotDeagleConfig : ShopItemConfig {
  public override int Price { get; init; } = 100;
}