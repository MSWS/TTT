using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs.Traitor;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Events.Body;
using TTT.Game.Events.Game;
using TTT.Game.Listeners;
using TTT.Shop.Events;

namespace TTT.Shop.Items.Traitor.Gloves;

public class GlovesListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private readonly GlovesConfig item =
    provider.GetService<IStorage<GlovesConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new GlovesConfig();

  private readonly Dictionary<IPlayer, int> uses = new();

  [UsedImplicitly]
  [EventHandler]
  public void OnPurchase(PlayerPurchaseItemEvent ev) {
    if (ev.Item is not GlovesItem) return;
    uses[ev.Player] = item.MaxUses;
  }

  [UsedImplicitly]
  [EventHandler]
  public void BodyCreate(BodyCreateEvent ev) {
    if (ev.Body.Killer == null || !useGloves(ev.Body.Killer)) return;
    if (ev.Body.Killer is not IOnlinePlayer online) return;
    ev.Body.Killer = null;
    Messenger.Message(online,
      Locale[
        GlovesMsgs.SHOP_ITEM_GLOVES_USED_KILL(uses[online], item.MaxUses)]);
  }

  [UsedImplicitly]
  [EventHandler]
  public void BodyIdentify(BodyIdentifyEvent ev) {
    if (ev.Identifier == null || !useGloves(ev.Identifier)) return;
    ev.IsCanceled = true;

    Messenger.Message(ev.Identifier,
      Locale[
        GlovesMsgs.SHOP_ITEM_GLOVES_USED_BODY(uses[ev.Identifier],
          item.MaxUses)]);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnGameState(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;
    uses.Clear();
  }

  private bool useGloves(IPlayer player) {
    uses.TryGetValue(player, out var useCount);
    if (useCount <= 0) return false;
    uses[player] = useCount - 1;
    if (useCount - 1 > 0) return true;
    if (player is not IOnlinePlayer online) return true;
    shop.RemoveItem<GlovesItem>(online);
    Messenger.Message(online, Locale[GlovesMsgs.SHOP_ITEM_GLOVES_WORN_OUT]);
    return true;
  }
}