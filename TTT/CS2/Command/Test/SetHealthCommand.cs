using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class SetHealthCommand : ICommand {
  public string Id => "sethealth";

  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    if (info.ArgCount != 2) return Task.FromResult(CommandResult.PRINT_USAGE);

    if (!int.TryParse(info.Args[1], out var health)) {
      info.ReplySync("Invalid health value.");
      return Task.FromResult(CommandResult.ERROR);
    }

    executor.Health = health;
    info.ReplySync($"Set health of {executor.Name} to {health}.");
    return Task.FromResult(CommandResult.SUCCESS);
  }
}