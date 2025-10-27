using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class SetTargetCommand(IServiceProvider provider) : ICommand {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);
    var gamePlayer = converter.GetPlayer(executor);
    if (gamePlayer == null) return Task.FromResult(CommandResult.ERROR);

    info.ReplySync("Target: " + gamePlayer.Target);
    gamePlayer.Target = "traitor";
    info.ReplySync("New Target: " + gamePlayer.Target);
    return Task.FromResult(CommandResult.SUCCESS);
  }
}