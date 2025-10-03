using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Events;
using TTT.API;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Shop;

public class Shop(IServiceProvider provider) : ITerrorModule, IShop {
  private readonly Dictionary<string, int> balances = new();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();
  private readonly Dictionary<string, List<IShopItem>> items = new();

  private readonly IMsgLocalizer localizer =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IMessenger? messenger = provider.GetService<IMessenger>();

  public Task<int> Load(IPlayer key) {
    return Task.FromResult(balances.GetValueOrDefault(key.Id, 0));
  }

  public ISet<IShopItem> Items { get; } = new HashSet<IShopItem>();

  public bool RegisterItem(IShopItem item) { return Items.Add(item); }

  public PurchaseResult TryPurchase(IOnlinePlayer player, IShopItem item,
    bool printReason = true) {
    var cost = item.Config.Price;
    var bal  = balances.GetValueOrDefault(player.Id, 0);

    if (cost > bal) {
      if (printReason)
        messenger?.Message(player,
          localizer[ShopMsgs.SHOP_INSUFFICIENT_BALANCE(item, bal)]);
      return PurchaseResult.INSUFFICIENT_FUNDS;
    }

    var canPurchase = item.CanPurchase(player);
    if (canPurchase != PurchaseResult.SUCCESS) {
      if (!printReason) return canPurchase;
      if (canPurchase == PurchaseResult.UNKNOWN_ERROR)
        messenger?.Message(player, localizer[ShopMsgs.SHOP_CANNOT_PURCHASE]);
      else
        messenger?.Message(player,
          localizer[
            ShopMsgs.SHOP_CANNOT_PURCHASE_WITH_REASON(
              canPurchase.ToMessage())]);

      return canPurchase;
    }

    var purchaseEvent = new PlayerPurchaseItemEvent(player, item);
    bus.Dispatch(purchaseEvent);
    if (purchaseEvent.IsCanceled) return PurchaseResult.PURCHASE_CANCELED;

    AddBalance(player, -cost, item.Name);
    GiveItem(player, item);

    if (printReason)
      messenger?.Message(player, localizer[ShopMsgs.SHOP_PURCHASED(item)]);
    return PurchaseResult.SUCCESS;
  }

  public void AddBalance(IOnlinePlayer player, int amount, string reason = "",
    bool print = true) {
    if (amount == 0) return;
    balances.TryAdd(player.Id, 0);

    var balEvent = new PlayerBalanceEvent(player, balances[player.Id],
      balances[player.Id] + amount, reason);

    bus.Dispatch(balEvent);

    if (balEvent.IsCanceled) return;
    messenger?.Debug(
      $"[Shop] {player.Name} ({player.Id}) balance changed from {balEvent.OldBalance} to {balEvent.NewBalance} "
      + $"({balEvent.NewBalance - balEvent.OldBalance})");
    balances[player.Id] = balEvent.NewBalance;

    if (!print || messenger == null) return;

    var msg = string.IsNullOrWhiteSpace(reason) ?
      ShopMsgs.CREDITS_GIVEN(amount) :
      ShopMsgs.CREDITS_GIVEN_REASON(amount, reason);
    messenger.Message(player, localizer[msg]);
  }

  public void ClearBalances() { balances.Clear(); }
  public void ClearItems() { items.Clear(); }

  public void GiveItem(IOnlinePlayer player, IShopItem item) {
    if (!items.ContainsKey(player.Id)) items[player.Id] = [];
    items[player.Id].Add(item);
    item.OnPurchase(player);
  }

  public IList<IShopItem> GetOwnedItems(IOnlinePlayer player) {
    return items.GetValueOrDefault(player.Id, []);
  }

  public void RemoveItem(IOnlinePlayer player, IShopItem item) {
    if (!items.TryGetValue(player.Id, out var itemList)) return;
    itemList.Remove(item);
  }

  public Task Write(IPlayer key, int newData) {
    balances[key.Id] = newData;
    return Task.CompletedTask;
  }

  public void Dispose() {
    foreach (var item in Items) item.Dispose();

    Items.Clear();
  }

  public void Start() { }
}