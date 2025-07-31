using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Game.Commands;

public class CommandManager(IServiceProvider provider) : ICommandManager {
  private readonly IMsgLocalizer localizer =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IPermissionManager permissions =
    provider.GetRequiredService<IPermissionManager>();

  private readonly Dictionary<string, ICommand> commands = new();

  public bool RegisterCommand(ICommand command)
    => command.Aliases.All(alias => commands.TryAdd(alias, command));

  public bool UnregisterCommand(ICommand command)
    => command.Aliases.All(alias => commands.Remove(alias));

  public bool CanExecute(IOnlinePlayer? executor, ICommand command) {
    if (executor == null) return true; // Allow all commands for console
    if (!permissions.HasFlags(executor, command.RequiredFlags)) return false;
    if (!permissions.InGroups(executor, command.RequiredGroups)) return false;
    return true;
  }

  public async Task<CommandResult> ProcessCommand(IOnlinePlayer? executor,
    ICommandInfo info) {
    if (info.ArgCount == 0) return CommandResult.ERROR;

    if (!commands.TryGetValue(info.Args[0], out var command)) {
      info.ReplySync(localizer[GameMsgs.GENERIC_UNKNOWN(info.Args[0])]);
      return CommandResult.UNKNOWN_COMMAND;
    }

    if (!CanExecute(executor, command)) {
      if (executor == null) {
        info.ReplySync(localizer[GameMsgs.GENERIC_NO_PERMISSION]);
        return CommandResult.NO_PERMISSION;
      }

      if (command.RequiredFlags.Any(f => !permissions.HasFlags(executor, f))) {
        info.ReplySync(localizer[
          GameMsgs.GENERIC_NO_PERMISSION_NODE(string.Join(", ",
            command.RequiredFlags))]);
        return CommandResult.NO_PERMISSION;
      }

      if (command.RequiredGroups.Any(g => permissions.InGroups(executor, g)))
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