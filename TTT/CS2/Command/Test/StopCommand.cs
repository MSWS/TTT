using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Game;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class StopCommand(IServiceProvider provider) : ICommand {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { }
  public string Name => "stop";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    Server.NextWorldUpdate(() => {
      if (!games.IsGameActive()) {
        info.ReplySync("No game is currently running.");
        return;
      }

      games.ActiveGame?.EndGame(
        new EndReason($"Force stopped by {executor?.Name ?? "Console"}"));
    });
    return Task.FromResult(CommandResult.SUCCESS);
  }
}