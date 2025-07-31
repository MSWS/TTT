using TTT.API.Player;

namespace TTT.API.Command;

public interface ICommand : IPluginModule {

  string? Description => null;
  string[] Usage => [];
  string[] RequiredFlags => [];
  string[] RequiredGroups => [];
  string[] Aliases => [Name];

  bool CanExecute(IOnlinePlayer? executor) {
    if (executor == null) return true;
    return RequiredFlags.All(flag => executor.HasFlags(flag)) 
        || RequiredGroups.Any(group => executor.InGroups(group));
  }

  Task<CommandResult> Execute(IOnlinePlayer? executor, ICommandInfo info);
}