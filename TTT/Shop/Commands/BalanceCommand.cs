using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Shop.Commands;

public class BalanceCommand(IServiceProvider provider) : ICommand {
  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public string Id => "balance";
  public string[] Aliases => [Id, "bal", "credits", "money", "points"];

  public void Dispose() { }
  public void Start() { }

  public async Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    if (executor == null) {
      info.ReplySync("You must be a player to check your balance.");
      return CommandResult.PLAYER_ONLY;
    }

    var bal = await shop.Load(executor);
    info.ReplySync(locale[ShopMsgs.COMMAND_BALANCE(bal)]);
    return CommandResult.SUCCESS;
  }
}