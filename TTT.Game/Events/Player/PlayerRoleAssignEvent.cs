namespace TTT.Api.Events.Player;

public class PlayerRoleAssignEvent : Event, ICancelableEvent {
  public override string Id => "core.event.player.roleassign";
  public bool IsCanceled { get; set; } = false;
}