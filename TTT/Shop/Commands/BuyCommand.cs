using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Command;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Shop.Commands;

public class BuyCommand(IServiceProvider provider) : ICommand {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private readonly IItemSorter? sorter = provider.GetService<IItemSorter>();

  public void Dispose() { }
  public string Id => "buy";
  public void Start() { }
  public string[] Aliases => [Id, "purchase", "b"];
  public string[] Usage => ["[item]"];

  public bool MustBeOnMainThread => true;

  public Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    if (games.ActiveGame is not { State: State.IN_PROGRESS }) {
      info.ReplySync(locale[ShopMsgs.SHOP_INACTIVE]);
      return Task.FromResult(CommandResult.SUCCESS);
    }

    if (info.ArgCount == 1) return Task.FromResult(CommandResult.PRINT_USAGE);

    if (executor.Health <= 0) {
      info.ReplySync(locale[ShopMsgs.SHOP_INACTIVE]);
      return Task.FromResult(CommandResult.SUCCESS);
    }

    var query = string.Join(" ", info.Args.Skip(1));
    var item  = searchItem(executor, query);

    if (item == null) {
      info.ReplySync(locale[ShopMsgs.SHOP_ITEM_NOT_FOUND(query)]);
      return Task.FromResult(CommandResult.ERROR);
    }

    var result = shop.TryPurchase(executor, item);
    return Task.FromResult(result == PurchaseResult.SUCCESS ?
      CommandResult.SUCCESS :
      CommandResult.ERROR);
  }

  private IShopItem? searchItem(IOnlinePlayer? player, string query) {
    if (sorter != null && int.TryParse(query, out var id)) {
      var items = sorter.GetSortedItems(player);
      if (id >= 0 && id < items.Count) return items[id];
      return null;
    }

    var searchSet = sortItems(player);

    var item = searchSet.FirstOrDefault(it
      => it.Name.Equals(query, StringComparison.OrdinalIgnoreCase));

    if (item != null) return item;

    item = searchSet.FirstOrDefault(it
      => it.Name.Contains(query, StringComparison.OrdinalIgnoreCase));

    if (item != null) return item;

    item = searchSet.FirstOrDefault(it
      => it.Description.Contains(query, StringComparison.OrdinalIgnoreCase));

    return item;
  }

  private List<IShopItem> sortItems(IOnlinePlayer? player) {
    var items = new List<IShopItem>(shop.Items).ToList();
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

    return items;
  }
}