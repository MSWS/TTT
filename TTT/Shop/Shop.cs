using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Command;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Locale;
using TTT.Shop.Commands;
using TTT.Shop.Items;

namespace TTT.Shop;

public class Shop(IServiceProvider provider) : ITerrorModule, IShop {
  private readonly Dictionary<IPlayer, int> balances = new();
  private readonly Dictionary<IPlayer, List<IShopItem>> items = new();

  private readonly IMsgLocalizer localizer =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IMessenger? messenger = provider.GetService<IMessenger>();

  public Task<int> Load(IPlayer key) {
    return Task.FromResult(balances.GetValueOrDefault(key, 0));
  }

  public ISet<IShopItem> Items { get; } = new HashSet<IShopItem>();

  public bool RegisterItem(IShopItem item) {
    item.Start();
    return Items.Add(item);
  }

  public PurchaseResult TryPurchase(IOnlinePlayer player, IShopItem item,
    bool printReason = true) {
    return PurchaseResult.UNKNOWN_ERROR;
  }

  public void AddBalance(IOnlinePlayer player, int amount, string reason = "",
    bool print = true) {
    if (amount == 0) return;
    balances.TryAdd(player, 0);
    balances[player] += amount;
    if (!print || messenger == null) return;

    var msg = string.IsNullOrWhiteSpace(reason) ?
      ShopMsgs.CREDITS_GIVEN(amount) :
      ShopMsgs.CREDITS_GIVEN_REASON(amount, reason);
    messenger.Message(player, localizer[msg]);
  }

  public void ClearBalances() { balances.Clear(); }
  public void ClearItems() { items.Clear(); }

  public void GiveItem(IOnlinePlayer player, IShopItem item) {
    if (!items.ContainsKey(player)) items[player] = [];
    items[player].Add(item);
  }

  public IList<IShopItem> GetOwnedItems(IOnlinePlayer player) {
    return items.GetValueOrDefault(player, []);
  }

  public void RemoveItem(IOnlinePlayer player, IShopItem item) {
    if (!items.TryGetValue(player, out var itemList)) return;
    itemList.Remove(item);
  }

  public Task Write(IPlayer key, int newData) {
    balances[key] = newData;
    return Task.CompletedTask;
  }

  public void Dispose() {
    foreach (var item in Items) { item.Dispose(); }

    Items.Clear();
  }

  public void Start() { RegisterItem(new OneShotDeagle(provider)); }
}