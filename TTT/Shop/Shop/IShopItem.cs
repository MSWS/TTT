using TTT.API;
using TTT.API.Player;

namespace TTT.Shop;

public interface IShopItem : ITerrorModule {
  string Name { get; }
  string Description { get; }
  ShopItemConfig Config { get; }
  void OnPurchase(IOnlinePlayer player);
  PurchaseResult CanPurchase(IOnlinePlayer player);
}