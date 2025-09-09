using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Game;
using TTT.Locale;

namespace TTT.Shop.Commands;

public class ShopCommand(IServiceProvider provider) : ICommand {
  private readonly IMsgLocalizer locale = provider
   .GetRequiredService<IMsgLocalizer>();

  private readonly Dictionary<string, ICommand> sub = new() {
    ["list"] = new ListCommand(provider)
  };

  public void Dispose() { }
  public string Name => "shop";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() {
    provider.GetRequiredService<ICommandManager>().RegisterCommand(this);
  }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    HashSet<string> sent = [];
    foreach (var (_, cmd) in sub) {
      if (!sent.Add(cmd.Name)) continue;
      var uses = cmd.Usage.Where(use => !string.IsNullOrWhiteSpace(use))
       .ToList();
      var useString = uses.Count > 0 ? "(" + string.Join(", ", uses) + ")" : "";
      if (cmd.Description != null)
        info.ReplySync(
          $"{locale[GameMsgs.PREFIX]}{ChatColors.White}{cmd.Name} {ChatColors.Grey}- {ChatColors.BlueGrey}{cmd.Description}");
      else
        info.ReplySync(
          $"{locale[GameMsgs.PREFIX]}{ChatColors.White}{cmd.Name} {ChatColors.Grey}{useString}");
    }

    return Task.FromResult(CommandResult.ERROR);
  }
}