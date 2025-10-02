using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.CS2.Items.DNA;

public static class DnaScannerServiceCollection {
  public static void AddDnaScannerServices(this IServiceCollection collection) {
    collection.AddModBehavior<DnaScanner>();
    collection.AddModBehavior<DnaListener>();
  }
}

public class DnaScanner(IServiceProvider provider)
  : RoleRestrictedItem<DetectiveRole>(provider) {
  private readonly DnaScannerConfig config = provider
   .GetService<IStorage<DnaScannerConfig>>()
  ?.Load()
   .GetAwaiter()
   .GetResult() ?? new DnaScannerConfig();

  public override string Name => Locale[DnaMsgs.SHOP_ITEM_DNA];
  public override string Description => Locale[DnaMsgs.SHOP_ITEM_DNA_DESC];
  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) { }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    if (Shop.HasItem(player, this)) return PurchaseResult.ALREADY_OWNED;
    return base.CanPurchase(player);
  }
}