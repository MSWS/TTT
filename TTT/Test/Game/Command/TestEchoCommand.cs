using TTT.API.Command;
using TTT.API.Player;

namespace TTT.Test.Game.Command;

public class TestEchoCommand : ICommand {
  public string Name => "echo";
  public string Version { get; }
  public void Start() { }
  public string[] Aliases => [ "echo", "say" ];

  public Task<CommandResult> Execute(IOnlinePlayer? executor, ICommandInfo info) {
    info.ReplySync(string.Join(' ', info.Args.Skip(1)));
    return Task.FromResult(CommandResult.SUCCESS);
  }

  public void Dispose() { }
}