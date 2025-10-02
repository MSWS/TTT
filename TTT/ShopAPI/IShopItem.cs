using ShopAPI.Configs;
using TTT.API;
using TTT.API.Player;

namespace ShopAPI;

public interface IShopItem : ITerrorModule {
  string Name { get; }
  string Description { get; }
  ShopItemConfig Config { get; }
  void OnPurchase(IOnlinePlayer player);
  PurchaseResult CanPurchase(IOnlinePlayer player);
}