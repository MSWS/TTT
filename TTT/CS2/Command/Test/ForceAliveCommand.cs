using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.CS2.API;

namespace TTT.CS2.Command.Test;

public class ForceAliveCommand(IServiceProvider provider) : ICommand {
  private readonly IAliveSpoofer spoofer =
    provider.GetRequiredService<IAliveSpoofer>();

  public void Dispose() { }

  public string Name => "forcealive";

  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    Server.NextWorldUpdate(() => {
      foreach (var player in Utilities.GetPlayers()) spoofer.SpoofAlive(player);
    });

    info.ReplySync("Attempted to force alive.");

    return Task.FromResult(CommandResult.SUCCESS);
  }
}