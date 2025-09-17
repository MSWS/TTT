using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.Shop.Commands;

public class BuyCommand(IServiceProvider provider) : ICommand {
  private readonly IShop shop = provider.GetRequiredService<IShop>();
  public void Dispose() { }
  public string Name => "buy";
  public void Start() { }
  public string[] Aliases => [Name, "purchase", "b"];

  public async Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    if (executor == null) {
      info.ReplySync("You must be a player to buy items.");
      return CommandResult.PLAYER_ONLY;
    }

    if (info.ArgCount == 1) {
      info.ReplySync("Please specify an item to buy.");
      return CommandResult.INVALID_ARGS;
    }

    var query = string.Join(" ", info.Args.Skip(1));
    info.ReplySync($"Searching for item: {query}");

    var item = searchItem(query);

    if (item == null) {
      info.ReplySync($"Item '{query}' not found.");
      return CommandResult.ERROR;
    }

    var bal = await shop.Load(executor);
    if (item.Config.Price > bal) {
      info.ReplySync(
        $"You cannot afford '{item.Name}'. It costs {item.Config.Price}, but you have {bal}.");
      return CommandResult.ERROR;
    }

    if (item.CanPurchase(executor) != PurchaseResult.SUCCESS) {
      info.ReplySync($"You cannot purchase '{item.Name}'.");
      return CommandResult.ERROR;
    }

    await shop.Write(executor, bal - item.Config.Price);
    item.OnPurchase(executor);
    shop.GiveItem(executor, item);
    return CommandResult.SUCCESS;
  }

  private IShopItem? searchItem(string query) {
    var item = shop.Items.FirstOrDefault(it
      => it.Id.Equals(query, StringComparison.OrdinalIgnoreCase));

    if (item != null) return item;

    item = shop.Items.FirstOrDefault(it
      => it.Name.Equals(query, StringComparison.OrdinalIgnoreCase));

    if (item != null) return item;

    item = shop.Items.FirstOrDefault(it
      => it.Name.Contains(query, StringComparison.OrdinalIgnoreCase));

    return item;
  }
}