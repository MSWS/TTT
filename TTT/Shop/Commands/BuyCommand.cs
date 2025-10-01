using Microsoft.Extensions.DependencyInjection;
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

  public async Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    if (executor == null) {
      info.ReplySync("You must be a player to buy items.");
      return CommandResult.PLAYER_ONLY;
    }

    if (games.ActiveGame is not { State: State.IN_PROGRESS }) {
      info.ReplySync(locale[ShopMsgs.SHOP_INACTIVE]);
      return CommandResult.SUCCESS;
    }

    if (info.ArgCount == 1) return CommandResult.PRINT_USAGE;

    var query = string.Join(" ", info.Args.Skip(1));
    info.ReplySync($"Searching for item: {query}");

    var item = searchItem(query);

    if (item == null) {
      info.ReplySync($"Item '{query}' not found.");
      return CommandResult.ERROR;
    }

    var result = shop.TryPurchase(executor, item);
    return result == PurchaseResult.SUCCESS ?
      CommandResult.SUCCESS :
      CommandResult.ERROR;
  }

  private IShopItem? searchItem(string query) {
    var item = shop.Items.FirstOrDefault(it
      => it.Name.Equals(query, StringComparison.OrdinalIgnoreCase));

    if (item != null) return item;

    item = shop.Items.FirstOrDefault(it
      => it.Name.Equals(query, StringComparison.OrdinalIgnoreCase));

    if (item != null) return item;

    item = shop.Items.FirstOrDefault(it
      => it.Name.Contains(query, StringComparison.OrdinalIgnoreCase));

    return item;
  }
}