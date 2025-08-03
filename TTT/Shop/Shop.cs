using TTT.API;
using TTT.API.Player;

namespace TTT.Shop;

public class Shop : ITerrorModule, IShop {
  private readonly Dictionary<IOnlinePlayer, int> balances = new();

  public Task<int> Load(IOnlinePlayer key) {
    return Task.FromResult(balances.GetValueOrDefault(key, 0));
  }

  public ISet<IShopItem> Items { get; } = new HashSet<IShopItem>();

  public bool RegisterItem(IShopItem item) { return Items.Add(item); }

  public PurchaseResult TryPurchase(IOnlinePlayer player, IShopItem item,
    bool printReason = true) {
    return PurchaseResult.UNKNOWN_ERROR;
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