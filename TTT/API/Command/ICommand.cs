using TTT.API.Player;

namespace TTT.API.Command;

public interface ICommand : ITerrorModule {
  string? Description => null;
  string[] Usage => [];
  string[] RequiredFlags => [];
  string[] RequiredGroups => [];
  string[] Aliases => [Id];
  bool MustBeOnMainThread => false;

  Task<CommandResult> Execute(IOnlinePlayer? executor, ICommandInfo info);
}