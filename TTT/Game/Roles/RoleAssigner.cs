using TTT.API;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.Game.Roles;

public class RoleAssigner(IEventBus bus, IOnlineMessenger onlineMessenger,
  IPlayerFinder finder) : IRoleAssigner {
  public void AssignRoles(ISet<IOnlinePlayer> players, IList<IRole> roles) {
    var  shuffled = players.OrderBy(_ => Guid.NewGuid()).ToHashSet();
    bool roleAssigned;
    do {
      roleAssigned = false;
      foreach (var role in roles) {
        var player = role.FindPlayerToAssign(shuffled);
        if (player is null) continue;

        var ev = new PlayerRoleAssignEvent(player, role);
        bus.Dispatch(ev);

        if (ev.IsCanceled) continue;

        player.Roles.Add(ev.Role);
        roleAssigned = true;

        onlineMessenger?.BackgroundMsgAll(finder,
          $"{player.Name} has been assigned the role of {role.Name}.");
      }
    } while (roleAssigned);
  }
}