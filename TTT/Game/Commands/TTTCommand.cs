using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Messages;
using TTT.API.Player;

namespace TTT.Game.Commands;

public class TTTCommand(IServiceProvider provider) : ICommand {
  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  public void Dispose() { }
  public string Name => "ttt";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    messenger.Message(executor,
      "[MSG] TTT Version: " + GitVersionInformation.FullSemVer);
    return Task.FromResult(CommandResult.SUCCESS);
  }
}