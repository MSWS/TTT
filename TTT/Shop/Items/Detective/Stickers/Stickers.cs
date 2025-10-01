using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.Game.Roles;
using TTT.Locale;

namespace TTT.Shop.Items.Detective.Stickers;

public static class StickerExtensions {
  public static void AddStickerServices(this IServiceCollection services) {
    services.AddModBehavior<Stickers>();
    services.AddModBehavior<StickerListener>();
  }
}

public class Stickers(IServiceProvider provider) : BaseItem(provider) {
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
    if (icons == null || !Roles.GetRoles(player).Any(r => r is DetectiveRole))
      return PurchaseResult.WRONG_ROLE;
    if (Shop.HasItem(player, this)) return PurchaseResult.ALREADY_OWNED;
    return PurchaseResult.SUCCESS;
  }
}

public record StickerConfig : ShopItemConfig {
  public override int Price { get; init; } = 70;
}