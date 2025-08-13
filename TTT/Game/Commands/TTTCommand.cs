using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Game.Commands;

public class TTTCommand(IServiceProvider provider) : ICommand {
  private readonly IMsgLocalizer localizer =
    provider.GetRequiredService<IMsgLocalizer>();

  public void Dispose() { }
  public string Name => "ttt";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
#if DEBUG
    info.ReplySync("DEBUG BUILD");
#endif
    info.ReplySync(
      localizer[GameMsgs.CMD_TTT(GitVersionInformation.FullSemVer)]);
    return Task.FromResult(CommandResult.SUCCESS);
  }
}