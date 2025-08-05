using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Player;

namespace TTT.Game.Roles;

public class RoleAssigner(IServiceProvider provider) : IRoleAssigner {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IMessenger? onlineMessenger =
    provider.GetService<IMessenger>();

  public void AssignRoles(ISet<IOnlinePlayer> players, IList<IRole> roles) {
    var  shuffled = players.OrderBy(_ => Guid.NewGuid()).ToHashSet();
    bool roleAssigned;
    do { roleAssigned = tryAssignRole(shuffled, roles); } while (roleAssigned);
  }

  private bool tryAssignRole(HashSet<IOnlinePlayer> players,
    IList<IRole> roles) {
    foreach (var role in roles) {
      var player = role.FindPlayerToAssign(players);
      if (player is null) continue;

      var ev = new PlayerRoleAssignEvent(player, role);
      bus.Dispatch(ev);

      if (ev.IsCanceled) continue;

      player.Roles.Add(ev.Role);
      ev.Role.OnAssign(player);

      onlineMessenger?.ScreenMsg(player,
        $"You were assigned the role of {role.Name}.");
      onlineMessenger?.BackgroundMsgAll(finder,
        $"{player.Name} was assigned the role of {role.Name}.");
      return true;
    }

    return false;
  }
}