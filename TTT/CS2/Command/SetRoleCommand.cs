using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Events;
using TTT.API.Player;
using TTT.CS2.Roles;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.CS2.Command;

public class SetRoleCommand(IServiceProvider provider) : ICommand {
  public void Dispose() { }

  public string Name => "setrole";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    info.ReplySync("Setting role to Traitor...");

    Server.NextWorldUpdate(() => {
      executor.Roles.Clear();
      var ev =
        new PlayerRoleAssignEvent(executor, new CS2TraitorRole(provider));
      bus.Dispatch(ev);
      if (ev.IsCanceled) {
        info.ReplySync("Role assignment was canceled.");
        return;
      }

      executor.Roles.Add(ev.Role);
      ev.Role.OnAssign(executor);
    });
    info.ReplySync("Role set to Traitor");
    return Task.FromResult(CommandResult.SUCCESS);
  }
}