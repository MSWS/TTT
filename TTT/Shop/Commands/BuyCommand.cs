using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.Shop.Commands;

public class BuyCommand(IServiceProvider provider) : ICommand {
  private readonly IShop shop = provider.GetRequiredService<IShop>();
  public void Dispose() { }
  public string Name => "buy";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) {
      info.ReplySync("You must be a player to buy items.");
      return Task.FromResult(CommandResult.PLAYER_ONLY);
    }

    if (info.ArgCount == 1) {
      info.ReplySync("Please specify an item to buy.");
      return Task.FromResult(CommandResult.INVALID_ARGS);
    }

    var query = string.Join(" ", info.Args.Skip(1));
    info.ReplySync($"Searching for item: {query}");

    var item = shop.Items.FirstOrDefault(it
      => it.Name.Equals(query, StringComparison.OrdinalIgnoreCase));

    if (item == null) {
      info.ReplySync($"Item '{query}' not found.");
      return Task.FromResult(CommandResult.ERROR);
    }

    if (!item.CanPurchase(executor)) {
      info.ReplySync($"You cannot purchase '{item.Name}'.");
      return Task.FromResult(CommandResult.ERROR);
    }

    item.OnPurchase(executor);
    shop.GiveItem(executor, item);
    return Task.FromResult(CommandResult.SUCCESS);
  }
}