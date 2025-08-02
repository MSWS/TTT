using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Messages;
using TTT.API.Player;

namespace TTT.CS2;

public class TTTCommand(IServiceProvider provider) : ICommand {
  public void Dispose() { }
  public string Name => "ttt";
  public string Version => GitVersionInformation.FullSemVer;

  private readonly IOnlineMessenger messenger =
    provider.GetRequiredService<IOnlineMessenger>();

  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    messenger.Message(executor, "");

    return Task.FromResult(CommandResult.SUCCESS);
  }
}