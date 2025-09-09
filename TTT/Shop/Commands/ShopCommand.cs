using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.Shop.Commands;

public class ShopCommand(IServiceProvider provider) : ICommand {
  private readonly Dictionary<string, ICommand> sub = new() {
    ["list"] = new ListCommand(provider)
  };

  public void Dispose() { }
  public string Name => "shop";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() {
    provider.GetRequiredService<ICommandManager>().RegisterCommand(this);
  }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    return Task.FromResult(CommandResult.ERROR);
  }
}