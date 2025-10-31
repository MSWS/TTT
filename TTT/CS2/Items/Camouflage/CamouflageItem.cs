using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;

namespace TTT.CS2.Items.Camouflage;

public static class CamoServiceCollection {
  public static void AddCamoServices(this IServiceCollection collection) {
    collection.AddModBehavior<CamouflageItem>();
  }
}

public class CamouflageItem(IServiceProvider provider) : BaseItem(provider) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private CamoConfig config
    => Provider.GetService<IStorage<CamoConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new CamoConfig();

  public override string Name => Locale[CamoMsgs.SHOP_ITEM_CAMO];
  public override string Description => Locale[CamoMsgs.SHOP_ITEM_CAMO_DESC];
  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) {
    Server.NextWorldUpdate(() => {
      var gamePlayer = converter.GetPlayer(player);
      var alpha      = (int)Math.Round(config.CamoVisibility * 255);
      gamePlayer?.SetColor(Color.FromArgb(alpha, Color.White));
    });
  }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    return Shop.HasItem<CamouflageItem>(player) ?
      PurchaseResult.ALREADY_OWNED :
      PurchaseResult.SUCCESS;
  }
}