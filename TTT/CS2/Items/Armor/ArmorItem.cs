using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;

namespace TTT.CS2.Items.Armor;

public static class ArmorItemServicesCollection {
  public static void AddArmorServices(this IServiceCollection collection) {
    collection.AddModBehavior<ArmorItem>();
  }
}

public class ArmorItem(IServiceProvider provider) : BaseItem(provider) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private ArmorConfig config
    => Provider.GetService<IStorage<ArmorConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new ArmorConfig();

  public override string Name => Locale[ArmorMsgs.SHOP_ITEM_ARMOR];
  public override string Description => Locale[ArmorMsgs.SHOP_ITEM_ARMOR_DESC];
  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) {
    var gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return;

    gamePlayer.SetArmor(config.Armor, config.Helmet);
  }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    return PurchaseResult.SUCCESS;
  }
}