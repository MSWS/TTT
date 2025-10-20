using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Detective;
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
  : RoleRestrictedItem<DetectiveRole>(provider) {
  private StickersConfig config
    => Provider.GetService<IStorage<StickersConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new StickersConfig();

  public override string Name => Locale[StickerMsgs.SHOP_ITEM_STICKERS];

  public override string Description
    => Locale[StickerMsgs.SHOP_ITEM_STICKERS_DESC];

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) { }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    if (Shop.HasItem<Stickers>(player)) return PurchaseResult.ALREADY_OWNED;
    return base.CanPurchase(player);
  }
}