using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Command;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Locale;

namespace TTT.Shop.Commands;

public class ListCommand(IServiceProvider provider) : ICommand, IItemSorter {
  private readonly IDictionary<string, List<IShopItem>> cache =
    new Dictionary<string, List<IShopItem>>();

  private readonly IGameManager games = provider
   .GetRequiredService<IGameManager>();

  private readonly IDictionary<string, DateTime> lastUpdate =
    new Dictionary<string, DateTime>();

  private readonly IMsgLocalizer locale = provider
   .GetRequiredService<IMsgLocalizer>();

  private readonly IRoleAssigner roles = provider
   .GetRequiredService<IRoleAssigner>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public void Dispose() { }

  public string Id => "list";

  public void Start() { }

  public async Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    var items = calculateSortedItems(executor);

    if (executor != null) cache[executor.Id] = items;
    items = new List<IShopItem>(items);
    items.Reverse();

    var balance = executor == null ? int.MaxValue : await shop.Load(executor);

    foreach (var (index, item) in items.Select((value, i) => (i, value))) {
      var canPurchase = executor == null
        || item.CanPurchase(executor) == PurchaseResult.SUCCESS;
      canPurchase = canPurchase && item.Config.Price <= balance;
      info.ReplySync(formatItem(item, items.Count - index, canPurchase));
    }

    if (games.ActiveGame is not { State: State.IN_PROGRESS }
      || executor == null)
      return CommandResult.SUCCESS;
    var role = roles.GetRoles(executor).FirstOrDefault();
    if (role == null) return CommandResult.SUCCESS;

    info.ReplySync(locale[ShopMsgs.SHOP_LIST_FOOTER(role, balance)]);
    return CommandResult.SUCCESS;
  }

  public List<IShopItem> GetSortedItems(IOnlinePlayer? player,
    bool refresh = false) {
    if (player == null) return calculateSortedItems(null);
    if (refresh || !cache.ContainsKey(player.Id))
      cache[player.Id] = calculateSortedItems(player);
    return cache[player.Id];
  }

  public DateTime? GetLastUpdate(IOnlinePlayer? player) {
    if (player == null) return null;
    lastUpdate.TryGetValue(player.Id, out var time);
    return time;
  }

  private List<IShopItem> calculateSortedItems(IOnlinePlayer? player) {
    var items = new List<IShopItem>(shop.Items).Where(item
        => player == null
        || games.ActiveGame is not { State: State.IN_PROGRESS }
        || item.CanPurchase(player) != PurchaseResult.WRONG_ROLE)
     .ToList();
    items.Sort((a, b) => {
      var aPrice = a.Config.Price;
      var bPrice = b.Config.Price;
      var aCanBuy = player != null
        && a.CanPurchase(player) == PurchaseResult.SUCCESS;
      var bCanBuy = player != null
        && b.CanPurchase(player) == PurchaseResult.SUCCESS;

      if (aCanBuy && !bCanBuy) return -1;
      if (!aCanBuy && bCanBuy) return 1;
      if (aPrice != bPrice) return aPrice.CompareTo(bPrice);
      return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
    });

    if (player != null) lastUpdate[player.Id] = DateTime.Now;
    return items;
  }

  private string formatPrefix(IShopItem item, int index, bool canBuy) {
    if (!canBuy)
      return
        $" {ChatColors.Grey}- [{ChatColors.DarkRed}{item.Config.Price}{ChatColors.Grey}] {ChatColors.Red}{item.Name}";

    if (index > 9)
      return
        $" {ChatColors.Default}- [{ChatColors.Yellow}{item.Config.Price}{ChatColors.Default}] {ChatColors.Green}{item.Name}";

    return
      $" {ChatColors.Blue}/{index} {ChatColors.Default}| [{ChatColors.Yellow}{item.Config.Price}{ChatColors.Default}] {ChatColors.Green}{item.Name}";
  }

  private string formatItem(IShopItem item, int index, bool canBuy) {
    return $" {formatPrefix(item, index, canBuy)}";
  }
}