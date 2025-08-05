using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Game.Commands;

public class CommandManager(IServiceProvider provider) : ICommandManager {
  protected readonly Dictionary<string, ICommand> Commands = new();

  protected readonly IMsgLocalizer Localizer =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IPermissionManager permissions =
    provider.GetRequiredService<IPermissionManager>();

  protected readonly IServiceProvider Provider = provider;

  public virtual bool RegisterCommand(ICommand command) {
    return command.Aliases.All(alias => Commands.TryAdd(alias, command));
  }

  public bool UnregisterCommand(ICommand command) {
    return command.Aliases.All(alias => Commands.Remove(alias));
  }

  public bool CanExecute(IOnlinePlayer? executor, ICommand command) {
    if (executor == null) return true; // Allow all commands for console
    if (!permissions.HasFlags(executor, command.RequiredFlags)) return false;
    if (!permissions.InGroups(executor, command.RequiredGroups)) return false;
    return true;
  }

  public async Task<CommandResult> ProcessCommand(ICommandInfo info) {
    var executor = info.CallingPlayer;
    if (info.ArgCount == 0) return CommandResult.ERROR;

    if (!Commands.TryGetValue(info.Args[0], out var command)) {
      info.ReplySync(Localizer[GameMsgs.GENERIC_UNKNOWN(info.Args[0])]);
      return CommandResult.UNKNOWN_COMMAND;
    }

    if (!CanExecute(executor, command)) {
      printNoPermission(executor, command, info);
      return CommandResult.NO_PERMISSION;
    }

    var result = await command.Execute(executor, info);

    switch (result) {
      case CommandResult.PLAYER_ONLY:
        info.ReplySync(Localizer[GameMsgs.GENERIC_PLAYER_ONLY]);
        break;
      case CommandResult.PRINT_USAGE: {
        foreach (var usage in command.Usage)
          info.ReplySync(
            Localizer[GameMsgs.GENERIC_USAGE($"{info.Args[0]} {usage}")]);
        break;
      }
    }

    return result;
  }

  private void printNoPermission(IOnlinePlayer? executor, ICommand command,
    ICommandInfo info) {
    if (executor == null) {
      info.ReplySync(Localizer[GameMsgs.GENERIC_NO_PERMISSION]);
      return;
    }

    if (command.RequiredFlags.Any(f => !permissions.HasFlags(executor, f))) {
      info.ReplySync(Localizer[
        GameMsgs.GENERIC_NO_PERMISSION_NODE(string.Join(", ",
          command.RequiredFlags))]);
      return;
    }

    info.ReplySync(Localizer[
      GameMsgs.GENERIC_NO_PERMISSION_RANK(string.Join(", ",
        command.RequiredGroups))]);
  }
}