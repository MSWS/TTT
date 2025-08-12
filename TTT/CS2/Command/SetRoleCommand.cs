using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.Roles;
using TTT.Game.Events.Player;

namespace TTT.CS2.Command;

public class SetRoleCommand(IServiceProvider provider) : ICommand {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();
  public void Dispose() { }

  public string Name => "setrole";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    IRole roleToAssign = new CS2TraitorRole(provider);
    if (info.ArgCount == 2)
      switch (info.Args[1].ToLowerInvariant()) {
        case "d" or "det" or "detective" or "ct":
          roleToAssign = new CS2DetectiveRole(provider);
          break;
        case "i" or "inn" or "innocent":
          roleToAssign = new CS2InnocentRole(provider);
          break;
      }

    Server.NextWorldUpdate(() => {
      executor.Roles.Clear();
      var ev = new PlayerRoleAssignEvent(executor, roleToAssign);
      bus.Dispatch(ev);
      if (ev.IsCanceled) {
        info.ReplySync("Role assignment was canceled.");
        return;
      }

      executor.Roles.Add(ev.Role);
      ev.Role.OnAssign(executor);
    });
    return Task.FromResult(CommandResult.SUCCESS);
  }
}