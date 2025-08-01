using TTT.API.Player;

namespace TTT.Shop;

public interface IShopItem {
  string Name { get; }
  int Price { get; }
  string Description { get; }
  void OnPurchase(IOnlinePlayer player);
}