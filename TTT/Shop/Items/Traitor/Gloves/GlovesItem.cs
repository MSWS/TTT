using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.Shop.Items.Traitor.Gloves;

public static class GlovesServiceCollection {
  public static void AddGlovesServices(this IServiceCollection collection) {
    collection.AddModBehavior<GlovesItem>();
    collection.AddModBehavior<GlovesListener>();
  }
}

public class GlovesItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider) {
  private readonly GlovesConfig config =
    provider.GetService<IStorage<GlovesConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new GlovesConfig();

  public override string Name => Locale[GlovesMsgs.SHOP_ITEM_GLOVES];

  public override string Description
    => Locale[GlovesMsgs.SHOP_ITEM_GLOVES_DESC];

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) { }
}