using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace ShopAPI.Events;

public class PlayerBalanceEvent(IPlayer player, int oldBalance, int newBalance,
  string? reason) : PlayerEvent(player), ICancelableEvent {
  public override string Id { get; } = "shop.event.player.balance";

  public int OldBalance { get; } = oldBalance;
  public int NewBalance { get; set; } = newBalance;
  public string? Reason { get; } = reason;
  public bool IsCanceled { get; set; }
}