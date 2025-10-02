using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.Shop.Items.Detective.Stickers;

public static class StickerExtensions {
  public static void AddStickerServices(this IServiceCollection services) {
    services.AddModBehavior<Stickers>();
    services.AddModBehavior<StickerListener>();
  }
}

public class Stickers(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider) {
  private readonly StickerConfig config = provider
   .GetService<IStorage<StickerConfig>>()
  ?.Load()
   .GetAwaiter()
   .GetResult() ?? new StickerConfig();

  private readonly IIconManager? icons = provider.GetService<IIconManager>();

  public override string Name => Locale[StickerMsgs.SHOP_ITEM_STICKERS];

  public override string Description
    => Locale[StickerMsgs.SHOP_ITEM_STICKERS_DESC];

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) { }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    if (Shop.HasItem(player, this)) return PurchaseResult.ALREADY_OWNED;
    return base.CanPurchase(player);
  }
}