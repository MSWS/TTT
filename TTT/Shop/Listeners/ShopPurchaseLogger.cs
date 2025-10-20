using JetBrains.Annotations;
using ShopAPI.Events;
using TTT.API.Events;
using TTT.Game.Listeners;
using TTT.Shop.Logs;

namespace TTT.Shop.Listeners;

public class ShopPurchaseLogger(IServiceProvider provider)
  : BaseListener(provider) {
  [UsedImplicitly]
  [EventHandler(Priority = Priority.MONITOR, IgnoreCanceled = true)]
  public void OnShopPurchase(PlayerPurchaseItemEvent ev) {
    Games.ActiveGame?.Logger.LogAction(new PurchaseAction(Roles, ev.Player,
      ev.Item));
  }
}