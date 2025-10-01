using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Game;
using TTT.API.Player;

namespace TTT.Game.Commands;

public class LogsCommand(IServiceProvider provider) : ICommand {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { }

  public string Id => "logs";
  public void Start() { }

  // TODO: Restrict and verbalize usage

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (games.ActiveGame is not {
      State: State.IN_PROGRESS or State.FINISHED
    }) {
      info.ReplySync("No active game to show logs for.");
      return Task.FromResult(CommandResult.ERROR);
    }

    games.ActiveGame.Logger.PrintLogs(executor);
    return Task.FromResult(CommandResult.SUCCESS);
  }
}