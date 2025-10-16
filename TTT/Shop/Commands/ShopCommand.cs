using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Game;
using TTT.Locale;

namespace TTT.Shop.Commands;

public class ShopCommand(IServiceProvider provider) : ICommand, IItemSorter {
  private readonly ListCommand listCmd = new(provider);

  private readonly IMsgLocalizer locale = provider
   .GetRequiredService<IMsgLocalizer>();

  private Dictionary<string, ICommand>? subcommands;

  public void Dispose() { }
  public string Id => "shop";
  public string[] Usage => ["list", "buy [item]", "balance"];

  public void Start() {
    subcommands = new Dictionary<string, ICommand> {
      ["list"]    = listCmd,
      ["buy"]     = new BuyCommand(provider),
      ["balance"] = new BalanceCommand(provider),
      ["bal"]     = new BalanceCommand(provider)
    };
  }

  public bool MustBeOnMainThread => true;

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    HashSet<string> sent = [];
    if (subcommands == null) {
      info.ReplySync(
        $"{locale[GameMsgs.PREFIX]}{ChatColors.Red}No subcommands available.");
      return Task.FromResult(CommandResult.ERROR);
    }

    if (info.ArgCount == 1) {
      foreach (var (_, cmd) in subcommands) {
        if (!sent.Add(cmd.Id)) continue;
        var uses = cmd.Usage.Where(use => !string.IsNullOrWhiteSpace(use))
         .ToList();
        var useString =
          uses.Count > 0 ? "(" + string.Join(", ", uses) + ")" : "";
        if (cmd.Description != null)
          info.ReplySync(
            $"{locale[GameMsgs.PREFIX]}{ChatColors.White}{cmd.Id} {ChatColors.Grey}- {ChatColors.BlueGrey}{cmd.Description}");
        else
          info.ReplySync(
            $"{locale[GameMsgs.PREFIX]}{ChatColors.White}{cmd.Id} {ChatColors.Grey}{useString}");
      }

      return Task.FromResult(CommandResult.SUCCESS);
    }

    var sub = info.Args[1].ToLowerInvariant();
    if (subcommands.TryGetValue(sub, out var command))
      return command.Execute(executor, info.Skip());
    return Task.FromResult(CommandResult.ERROR);
  }

  public List<IShopItem> GetSortedItems(IOnlinePlayer? player,
    bool refresh = false) {
    return listCmd.GetSortedItems(player, refresh);
  }

  public DateTime? GetLastUpdate(IOnlinePlayer? player) {
    return listCmd.GetLastUpdate(player);
  }
}