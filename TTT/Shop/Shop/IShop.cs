using TTT.API.Player;
using TTT.API.Storage;

namespace TTT.Shop;

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
  void RemoveItem(IOnlinePlayer player, IShopItem item);
}