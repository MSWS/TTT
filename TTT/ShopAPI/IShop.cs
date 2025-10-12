using TTT.API.Player;
using TTT.API.Storage;

namespace ShopAPI;

public interface IShop : IKeyedStorage<IPlayer, int>,
  IKeyWritable<IPlayer, int> {
  ISet<IShopItem> Items { get; }

  bool RegisterItem(IShopItem item);

  PurchaseResult TryPurchase(IOnlinePlayer player, IShopItem item,
    bool printReason = true);

  void AddBalance(IOnlinePlayer player, int amount, string reason = "",
    bool print = true);

  void ClearBalances();
  void ClearItems();

  void GiveItem(IOnlinePlayer player, IShopItem item);
  IList<IShopItem> GetOwnedItems(IOnlinePlayer player);

  bool HasItem<T>(IOnlinePlayer player) where T : IShopItem {
    return GetOwnedItems(player).Any(i => i is T);
  }

  void RemoveItem<T>(IOnlinePlayer player) where T : IShopItem;
}