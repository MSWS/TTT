using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.API;
using TTT.CS2.Extensions;
using TTT.Game.Events.Body;
using TTT.Game.Listeners;

namespace TTT.CS2.Items.BodyPaint;

public class BodyPaintListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IBodyTracker bodies =
    provider.GetRequiredService<IBodyTracker>();

  private BodyPaintConfig config
    => Provider.GetService<IStorage<BodyPaintConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new BodyPaintConfig();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private readonly Dictionary<IPlayer, int> uses = new();

  [UsedImplicitly]
  [EventHandler(Priority = Priority.HIGH)]
  public void BodyIdentify(BodyIdentifyEvent ev) {
    if (!bodies.Bodies.TryGetValue(ev.Body, out var body)) return;
    if (ev.Identifier == null || !usePaint(ev.Identifier)) return;
    ev.IsCanceled = true;
    body.SetColor(config.ColorToApply);
  }

  private bool usePaint(IPlayer player) {
    if (player is not IOnlinePlayer online) return false;
    if (!uses.ContainsKey(player)) {
      if (!shop.HasItem<BodyPaintItem>(online)) return false;
      uses[player] = config.MaxUses;
    }

    if (uses[player] <= 0) return false;
    uses[player]--;
    if (uses[player] > 0) return true;
    shop.RemoveItem<BodyPaintItem>(online);
    Messenger.Message(online, Locale[BodyPaintMsgs.SHOP_ITEM_BODY_PAINT_OUT]);
    uses.Remove(player);
    return true;
  }
}