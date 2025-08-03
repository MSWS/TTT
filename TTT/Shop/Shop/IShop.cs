using TTT.API.Player;
using TTT.API.Storage;

namespace TTT.Shop;

public interface IShop : IKeyedStorage<IOnlinePlayer, int>,
  IKeyWritable<IOnlinePlayer, int> {
  ISet<IShopItem> Items { get; }

  bool RegisterItem(IShopItem item);

  PurchaseResult TryPurchase(IOnlinePlayer player, IShopItem item,
    bool printReason = true);

  void AddBalance(IOnlinePlayer player, int amount, string reason = "",
    bool print = true);

  void ClearBalances();
}