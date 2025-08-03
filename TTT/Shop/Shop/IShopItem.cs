using TTT.API.Player;

namespace TTT.Shop;

public interface IShopItem {
  string Name { get; }
  string Description { get; }
  ShopItemConfig Config { get; }
  void OnPurchase(IOnlinePlayer player);
  bool CanPurchase(IOnlinePlayer player);
}