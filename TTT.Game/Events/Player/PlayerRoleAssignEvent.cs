using TTT.Api;
using TTT.Api.Events;
using TTT.Api.Player;

namespace TTT.Game.Events.Player;

public class PlayerRoleAssignEvent(IPlayer player, IRole role)
  : PlayerEvent(player), ICancelableEvent {
  public override string Id => "basegame.event.player.roleassign";
  public bool IsCanceled { get; set; } = false;

  /// <summary>
  /// Generally not recommended to set this property directly.
  /// </summary>
  public IRole Role { get; internal set; } = role;
}