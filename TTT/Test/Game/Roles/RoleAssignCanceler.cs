using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Test.Game.Roles;

public class RoleAssignCanceler(IEventBus bus) : IListener {
  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler]
  public void OnPlayerRoleAssign(PlayerRoleAssignEvent ev) {
    if (ev.Role is TestRoles.RoleA) ev.IsCanceled = true;
  }
}