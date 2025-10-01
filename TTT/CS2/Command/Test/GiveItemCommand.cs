using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Shop;

namespace TTT.CS2.Command.Test;

public class GiveItemCommand(IServiceProvider provider) : ICommand {
  private readonly IShop shop = provider.GetRequiredService<IShop>();
  public void Dispose() { }
  public void Start() { }

  public string Id => "giveitem";

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    if (info.ArgCount == 1) return Task.FromResult(CommandResult.PRINT_USAGE);

    var query = string.Join(" ", info.Args.Skip(1));
    info.ReplySync($"Searching for item: {query}");
    var item = searchItem(query);
    if (item == null) {
      info.ReplySync($"Item '{query}' not found.");
      return Task.FromResult(CommandResult.ERROR);
    }

    shop.GiveItem(executor, item);
    info.ReplySync($"Gave item '{item.Name}' to {executor.Name}.");
    return Task.FromResult(CommandResult.SUCCESS);
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