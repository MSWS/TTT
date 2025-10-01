using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Player;
using TTT.CS2.API;
using TTT.CS2.Events;
using TTT.Game.Listeners;

namespace TTT.CS2.Items.DNA;

public class DnaListener(IServiceProvider provider) : BaseListener(provider) {
  private readonly IBodyTracker bodies =
    provider.GetRequiredService<IBodyTracker>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  // Low priority to allow body identification to happen first
  [EventHandler(Priority = Priority.LOW)]
  public void OnPropPickup(PropPickupEvent ev) {
    if (ev.Player is not IOnlinePlayer player) return;
    if (!shop.HasItem<DnaScanner>(player)) return;

    if (!bodies.TryLookup(ev.Prop.Index.ToString(), out var body)) return;
    if (body == null) return;

    Messenger.Message(player, $"Body: {body.Killer?.Name ?? "Unknown"}");
  }
}