using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Game.Commands;

public class CommandManager(IServiceProvider provider)
  : ICommandManager {
  private readonly IMsgLocalizer? localizer =
    provider.GetRequiredService<IMsgLocalizer>();
  
  protected readonly Dictionary<string, ICommand> Commands = new();
  
  public virtual bool RegisterCommand(ICommand command)
    => command.Aliases.All(alias => Commands.TryAdd(alias, command));

  public bool UnregisterCommand(ICommand command)
    => command.Aliases.All(alias => Commands.Remove(alias));

  public Task<CommandResult> ProcessCommand(IOnlinePlayer? executor,
    ICommandInfo info, params string[] args) {
    throw new NotImplementedException();
  }

  public virtual async Task<CommandResult> ProcessCommand(
    IOnlinePlayer? executor, ICommandInfo info) {
    if (info.ArgCount == 0) return CommandResult.ERROR;

    if (!Commands.TryGetValue(info.Args[0], out var command)) {
      info.ReplySync(localizer[GameMsgs.GENERIC_UNKNOWN(info.Args[0])]);
      return CommandResult.UNKNOWN_COMMAND;
    }

    if (!command.CanExecute(executor)) {
      if (executor == null) {
        info.ReplySync(localizer[GameMsgs.GENERIC_NO_PERMISSION]);
        return CommandResult.NO_PERMISSION;
      }

      if (command.RequiredFlags.Any(f => !executor.HasFlags(f))) {
        info.ReplySync(localizer[
          GameMsgs.GENERIC_NO_PERMISSION_NODE(string.Join(", ",
            command.RequiredFlags))]);
        return CommandResult.NO_PERMISSION;
      }

      if (command.RequiredGroups.Any(g => executor.InGroups(g)))
        return CommandResult.NO_PERMISSION;
      info.ReplySync(localizer[
        GameMsgs.GENERIC_NO_PERMISSION_RANK(string.Join(", ",
          command.RequiredGroups))]);
      return CommandResult.NO_PERMISSION;
    }

    var result = await command.Execute(executor, info);

    if (result == CommandResult.PLAYER_ONLY)
      info.ReplySync(localizer[GameMsgs.GENERIC_PLAYER_ONLY]);

    if (result == CommandResult.PRINT_USAGE) {
      foreach (var usage in command.Usage)
        info.ReplySync(
          localizer[GameMsgs.GENERIC_USAGE($"{info.Args[0]} {usage}")]);
    }

    return result;
  }
}