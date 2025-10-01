using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Command;
using TTT.API.Messages;
using TTT.API.Player;

namespace TTT.Shop.Commands;

public class ListCommand(IServiceProvider provider) : ICommand {
  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public void Dispose() { }

  public string Id => "list";

  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    foreach (var item in shop.Items)
      messenger.Message(executor, $"{item.Name} - {item.Description}");

    return Task.FromResult(CommandResult.SUCCESS);
  }
}