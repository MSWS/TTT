using TTT.API.Player;

namespace TTT.API.Command;

/// <summary>
///   An interface that allows for registering and processing commands.
/// </summary>
public interface ICommandManager {
  /// <summary>
  ///   Registers a command with the manager.
  /// </summary>
  /// <param name="command">True if the command was successfully registered.</param>
  bool RegisterCommand(ICommand command);

  /// <summary>
  ///   Unregisters a command from the manager.
  /// </summary>
  /// <param name="command">True if the command was successfully unregistered.</param>
  bool UnregisterCommand(ICommand command);

  bool CanExecute(IOnlinePlayer? executor,
    ICommand command);
  
  /// <summary>
  ///   Attempts to process a command.
  /// </summary>
  /// <param name="executor"></param>
  /// <param name="info"></param>
  /// <returns>True if the command finished processing successfully.</returns>
  Task<CommandResult> ProcessCommand(IOnlinePlayer? executor,
    ICommandInfo info);
}