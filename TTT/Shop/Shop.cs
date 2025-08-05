using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Shop;

public class Shop(IServiceProvider provider) : ITerrorModule, IShop {
  private readonly Dictionary<IOnlinePlayer, int> balances = new();

  private readonly IMsgLocalizer localizer =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IMessenger? messenger = provider.GetService<IMessenger>();

  public Task<int> Load(IOnlinePlayer key) {
    return Task.FromResult(balances.GetValueOrDefault(key, 0));
  }

  public ISet<IShopItem> Items { get; } = new HashSet<IShopItem>();

  public bool RegisterItem(IShopItem item) { return Items.Add(item); }

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

  public Task Write(IOnlinePlayer key, int newData) {
    balances[key] = newData;
    return Task.CompletedTask;
  }

  public void Dispose() { }
  public string Name => "TTT.Shop";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }
}