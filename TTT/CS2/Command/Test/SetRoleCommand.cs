using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.CS2.Command.Test;

public class SetRoleCommand(IServiceProvider provider) : ICommand {
  private readonly IRoleAssigner assigner =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  public void Dispose() { }

  public string Id => "setrole";
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    // IOnlinePlayer targetPlayer = executor;
    List<IOnlinePlayer> targets      = [executor];
    var                 targetName   = executor.Name;
    IRole               roleToAssign = new TraitorRole(provider);
    if (info.ArgCount == 2)
      switch (info.Args[1].ToLowerInvariant()) {
        case "d" or "det" or "detective" or "ct":
          roleToAssign = new DetectiveRole(provider);
          break;
        case "i" or "inn" or "innocent":
          roleToAssign = new InnocentRole(provider);
          break;
        default:
          targets = finder.GetMulti(info.Args[1], out targetName, executor);
          break;
      }

    if (info.ArgCount == 3) {
      targets = finder.GetMulti(info.Args[2], out targetName, executor);
    }

    Server.NextWorldUpdate(() => {
      foreach (var player in targets) {
        var ev = new PlayerRoleAssignEvent(player, roleToAssign);
        bus.Dispatch(ev);
        if (ev.IsCanceled) {
          info.ReplySync("Role assignment was canceled.");
          return;
        }

        assigner.Write(player, [ev.Role]);
        ev.Role.OnAssign(player);
      }

      info.ReplySync(
        "Assigned " + roleToAssign.Name + " to " + targetName + ".");
    });
    return Task.FromResult(CommandResult.SUCCESS);
  }
}