using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Events;
using TTT.API.Command;
using TTT.API.Events;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class GiveItemCommand(IServiceProvider provider) : ICommand {
  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public void Dispose() { }
  public void Start() { }

  public string Id => "giveitem";
  public string[] Usage => ["[item] <player>"];

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    if (info.ArgCount == 1) return Task.FromResult(CommandResult.PRINT_USAGE);

    var query = info.Args[1];
    var item  = searchItem(query);
    if (item == null) {
      info.ReplySync($"Item '{query}' not found.");
      return Task.FromResult(CommandResult.ERROR);
    }

    var target = executor;

    Server.NextWorldUpdateAsync(() => {
      if (info.ArgCount == 3) {
        var result = finder.GetPlayerByName(info.Args[2]);
        if (result == null) {
          info.ReplySync($"Player '{info.Args[2]}' not found.");
          return;
        }

        target = result;
      }

      var purchaseEv = new PlayerPurchaseItemEvent(target, item);
      provider.GetRequiredService<IEventBus>().Dispatch(purchaseEv);

      shop.GiveItem(target, item);
      info.ReplySync($"Gave item '{item.Name}' to {target.Name}.");
    });
    return Task.FromResult(CommandResult.SUCCESS);
  }

  private IShopItem? searchItem(string query) {
    var item = shop.Items.FirstOrDefault(it
      => it.Name.Replace(" ", "")
       .Equals(query, StringComparison.OrdinalIgnoreCase));

    if (item != null) return item;

    item = shop.Items.FirstOrDefault(it
      => it.Name.Equals(query, StringComparison.OrdinalIgnoreCase));

    if (item != null) return item;

    item = shop.Items.FirstOrDefault(it
      => it.Name.Contains(query, StringComparison.OrdinalIgnoreCase));

    return item;
  }
}