using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Command;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Game.Roles;

namespace TTT.Shop.Commands;

public class ListCommand(IServiceProvider provider) : ICommand {
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private readonly IGameManager games = provider
   .GetRequiredService<IGameManager>();

  public void Dispose() { }

  public string Id => "list";

  public void Start() { }

  public async Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    var items = new List<IShopItem>(shop.Items).Where(item
        => executor == null
        || games.ActiveGame is not { State: State.IN_PROGRESS }
        || item.CanPurchase(executor) != PurchaseResult.WRONG_ROLE)
     .ToList();

    items.Sort((a, b) => {
      var aPrice = a.Config.Price;
      var bPrice = b.Config.Price;
      var aCanBuy = executor != null
        && a.CanPurchase(executor) == PurchaseResult.SUCCESS;
      var bCanBuy = executor != null
        && b.CanPurchase(executor) == PurchaseResult.SUCCESS;

      if (aCanBuy && !bCanBuy) return -1;
      if (!aCanBuy && bCanBuy) return 1;
      if (aPrice != bPrice) return aPrice.CompareTo(bPrice);
      return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
    });

    var balance = info.CallingPlayer == null ?
      int.MaxValue :
      await shop.Load(info.CallingPlayer);

    var longestShopName = items.Select(item => formatPrefix(item).Length).Max();

    foreach (var item in items) {
      info.ReplySync(formatItem(item,
        item.Config.Price <= balance
        && item.CanPurchase(info.CallingPlayer ?? executor!)
        == PurchaseResult.SUCCESS, longestShopName));
    }

    return CommandResult.SUCCESS;
  }

  private string formatPrefix(IShopItem item, bool canBuy = true) {
    if (!canBuy)
      return
        $" {ChatColors.Grey}- [{ChatColors.DarkRed}{item.Config.Price}{ChatColors.Grey}] {ChatColors.Red}{item.Name}";
    return
      $" {ChatColors.Default}- [{ChatColors.Yellow}{item.Config.Price}{ChatColors.Grey}] {ChatColors.Green}{item.Name}";
  }

  private string formatItem(IShopItem item, bool canBuy, int longestShopName) {
    var paddedPrefix = formatPrefix(item, canBuy).PadRight(longestShopName);
    return $" {paddedPrefix} {ChatColors.Grey} | {item.Description}";
  }
}