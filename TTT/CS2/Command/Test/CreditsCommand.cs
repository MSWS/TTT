using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class CreditsCommand(IServiceProvider provider) : ICommand {
  private readonly IShop shop = provider.GetRequiredService<IShop>();
  public void Dispose() { }
  public void Start() { }
  public string Id => "credits";

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);
    shop.AddBalance(executor, 1000);
    info.ReplySync("You have been given 1000 credits!");
    return Task.FromResult(CommandResult.SUCCESS);
  }
}