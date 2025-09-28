using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Game;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class StateCommand(IServiceProvider provider) : ICommand {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public string Name => "state";
  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (games.ActiveGame == null) {
      info.ReplySync("ActiveGame is null.");
      return Task.FromResult(CommandResult.SUCCESS);
    }

    info.ReplySync($"Current game state: {games.ActiveGame?.State}");
    return Task.FromResult(CommandResult.SUCCESS);
  }
}