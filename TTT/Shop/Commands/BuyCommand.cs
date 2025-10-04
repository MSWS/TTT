using CounterStrikeSharp.API;
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

  public void Dispose() { }
  public string Id => "buy";
  public void Start() { }
  public string[] Aliases => [Id, "purchase", "b"];
  public string[] Usage => ["[item]"];

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
    var item  = searchItem(query);

    if (item == null) {
      info.ReplySync(locale[ShopMsgs.SHOP_ITEM_NOT_FOUND(query)]);
      return Task.FromResult(CommandResult.ERROR);
    }

    var result = shop.TryPurchase(executor, item);
    return Task.FromResult(result == PurchaseResult.SUCCESS ?
      CommandResult.SUCCESS :
      CommandResult.ERROR);
  }

  private IShopItem? searchItem(string query) {
    var item = shop.Items.FirstOrDefault(it
      => it.Name.Equals(query, StringComparison.OrdinalIgnoreCase));

    if (item != null) return item;

    item = shop.Items.FirstOrDefault(it
      => it.Name.Equals(query, StringComparison.OrdinalIgnoreCase));

    if (item != null) return item;

    item = shop.Items.FirstOrDefault(it
      => it.Description.Contains(query, StringComparison.OrdinalIgnoreCase));

    return item;
  }
}