using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Locale;

namespace TTT.Shop.Items;

public static class DeagleServiceCollection {
  public static void AddDeagleServices(this IServiceCollection collection) {
    collection.AddModBehavior<DeagleDamageListener>();
  }
}

public class OneShotDeagle(IServiceProvider provider)
  : BaseItem(provider), IWeapon {
  private readonly OneShotDeagleConfig deagleConfigStorage = provider
   .GetService<IStorage<OneShotDeagleConfig>>()
  ?.Load()
   .GetAwaiter()
   .GetResult() ?? new OneShotDeagleConfig();

  public override string Name => Locale[DeagleMsgs.SHOP_ITEM_DEAGLE];

  public override string Description
    => Locale[DeagleMsgs.SHOP_ITEM_DEAGLE_DESC];

  public override ShopItemConfig Config => deagleConfigStorage;

  public override void OnPurchase(IOnlinePlayer player) {
    Task.Run(async () => {
      await Inventory.RemoveWeaponInSlot(player,
        deagleConfigStorage.WeaponSlot);
      await Inventory.GiveWeapon(player, this);
    });
  }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    return PurchaseResult.SUCCESS;
  }

  public string WeaponId => deagleConfigStorage.Weapon;

  public int? ReserveAmmo { get; init; } = 0;
  public int? CurrentAmmo { get; init; } = 1;
}

public record OneShotDeagleConfig : ShopItemConfig {
  public override int Price { get; init; } = 100;
  public bool DoesFriendlyFire { get; init; } = true;
  public bool KillShooterOnFF { get; init; } = false;
  public string Weapon { get; init; } = "revolver";
  public int WeaponSlot { get; init; } = 1;
}