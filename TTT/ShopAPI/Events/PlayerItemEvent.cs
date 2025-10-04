using TTT.API.Player;
using TTT.Game.Events.Player;

namespace ShopAPI.Events;

public abstract class PlayerItemEvent(IPlayer player, IShopItem item)
  : PlayerEvent(player) {
  public IShopItem Item { get; } = item;
}