using TTT.API.Player;

namespace TTT.API.Command;

public interface ICommand : IPluginModule {
  string? Description => null;
  string[] Usage => [];
  string[] RequiredFlags => [];
  string[] RequiredGroups => [];
  string[] Aliases => [Name];

  bool CanExecute(IOnlinePlayer? executor);

  Task<CommandResult> Execute(IOnlinePlayer? executor, ICommandInfo info);
}