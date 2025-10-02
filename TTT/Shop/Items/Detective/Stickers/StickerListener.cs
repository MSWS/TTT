using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.Shop.Items.Detective.Stickers;

public class StickerListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IIconManager? icons = provider.GetService<IIconManager>();
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [EventHandler(Priority = Priority.MONITOR)]
  public void OnHurt(PlayerDamagedEvent ev) {
    if (icons == null || ev.Attacker == null
      || !shop.HasItem<Stickers>(ev.Attacker))
      return;
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    if (ev.Weapon == null) return;
    if (!ev.Weapon.Contains("taser", StringComparison.OrdinalIgnoreCase))
      return;
    if (!ev.IsCanceled) return;

    var victim   = ev.Player;
    var attacker = ev.Attacker;

    if (attacker == null) return;

    var player = converter.GetPlayer(victim);
    if (player == null || !player.IsValid) return;
    icons.RevealToAll(player.Slot);
    Messenger.Message(victim, Locale[StickerMsgs.SHOP_ITEM_STICKERS_HIT]);
  }
}