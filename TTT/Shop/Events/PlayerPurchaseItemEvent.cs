using TTT.API.Events;
using TTT.API.Player;

namespace TTT.Shop.Events;

public class PlayerPurchaseItemEvent(IPlayer player, IShopItem item)
  : PlayerItemEvent(player, item), ICancelableEvent {
  public override string Id { get; } = "shop.event.player.purchaseitem";
  public bool IsCanceled { get; set; }
}