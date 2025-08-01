using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Game.Events.Player;

public class PlayerRoleAssignEvent(IPlayer player, IRole role)
  : PlayerEvent(player), ICancelableEvent {
  public override string Id => "basegame.event.player.roleassign";

  /// <summary>
  ///   Generally not recommended to set this property directly.
  /// </summary>
  public IRole Role { get; internal set; } = role;

  public bool IsCanceled { get; set; }
}