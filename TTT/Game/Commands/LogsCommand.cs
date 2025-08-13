using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Game;
using TTT.API.Player;

namespace TTT.Game.Commands;

public class LogsCommand(IServiceProvider provider) : ICommand {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { }

  public string Name => "logs";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (!games.IsGameActive()) {
      info.ReplySync("No active game to show logs for.");
      return Task.FromResult(CommandResult.ERROR);
    }

    var game = games.ActiveGame;
    if (game == null) {
      info.ReplySync("No active game to show logs for.");
      return Task.FromResult(CommandResult.ERROR);
    }

    game.Logger.PrintLogs(executor);
    return Task.FromResult(CommandResult.SUCCESS);
  }
}