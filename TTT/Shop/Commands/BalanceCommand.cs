using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.Shop.Commands;

public class BalanceCommand(IServiceProvider provider) : ICommand {
  private readonly IShop shop = provider.GetRequiredService<IShop>();
  public string Name => "balance";
  public string[] Aliases => [Name, "bal", "credits", "money"];

  public void Dispose() { }
  public void Start() { }

  public async Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    if (executor == null) {
      info.ReplySync("You must be a player to check your balance.");
      return CommandResult.PLAYER_ONLY;
    }

    var bal = await shop.Load(executor);
    info.ReplySync($"You have {bal} credits.");
    return CommandResult.SUCCESS;
  }
}