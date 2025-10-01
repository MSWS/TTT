using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.CS2.API;

namespace TTT.CS2.Command.Test;

public class ShowIconsCommand(IServiceProvider provider) : ICommand {
  private readonly IIconManager icons =
    provider.GetRequiredService<IIconManager>();

  public string Id => "showicons";

  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    Server.NextWorldUpdate(() => {
      for (var i = 0; i < Server.MaxPlayers; i++)
        icons.SetVisiblePlayers(i, ulong.MaxValue);
    });

    info.ReplySync("Set all icons visible");
    return Task.FromResult(CommandResult.SUCCESS);
  }
}