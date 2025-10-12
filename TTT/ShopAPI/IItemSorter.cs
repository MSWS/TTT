using TTT.API.Player;

namespace ShopAPI;

public interface IItemSorter {
  List<IShopItem> GetSortedItems(IOnlinePlayer? player, bool refresh = false);
  DateTime? GetLastUpdate(IOnlinePlayer? player);
}