using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.CS2.Items.BodyPaint;

public static class BodyPaintServicesCollection {
  public static void AddBodyPaintServices(this IServiceCollection collection) {
    collection.AddModBehavior<BodyPaintItem>();
    collection.AddModBehavior<BodyPaintListener>();
  }
}

public class BodyPaintItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider) {
  private BodyPaintConfig config
    => Provider.GetService<IStorage<BodyPaintConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new BodyPaintConfig();

  public override string Name => Locale[BodyPaintMsgs.SHOP_ITEM_BODY_PAINT];

  public override string Description
    => Locale[BodyPaintMsgs.SHOP_ITEM_BODY_PAINT_DESC];

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) { }
}