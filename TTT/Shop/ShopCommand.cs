using TTT.API.Command;
using TTT.API.Player;

namespace TTT.Shop;

public class ShopCommand(IServiceProvider provider) : ICommand {
  public void Dispose() { }
  public string Name => "shop";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  private readonly Dictionary<string, ICommand> subCommands = new();

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    return Task.FromResult(CommandResult.ERROR);
  }
}