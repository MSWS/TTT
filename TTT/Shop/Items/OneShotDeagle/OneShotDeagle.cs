using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
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

public class OneShotDeagle(IServiceProvider provider) : IWeapon, IShopItem {
  private readonly OneShotDeagleConfig deagleConfigStorage =
    provider.GetService<IStorage<OneShotDeagleConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new OneShotDeagleConfig();

  private readonly IInventoryManager inventoryManager =
    provider.GetRequiredService<IInventoryManager>();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  public string Name => locale[DeagleMsgs.SHOP_ITEM_DEAGLE];

  public void Start() { }

  public void Dispose() { }
  public string Description => locale[DeagleMsgs.SHOP_ITEM_DEAGLE_DESC];

  public ShopItemConfig Config => deagleConfigStorage;

  public void OnPurchase(IOnlinePlayer player) {
    inventoryManager.GiveWeapon(player, this);
  }

  public PurchaseResult CanPurchase(IOnlinePlayer player) {
    return PurchaseResult.SUCCESS;
  }

  public string Id => deagleConfigStorage.Weapon;
  string IShopItem.Id => ID;

  public const string ID = "ttt.shop.item.oneshotdeagle";

  public int? ReserveAmmo { get; init; } = 0;
  public int? CurrentAmmo { get; init; } = 1;
}

public record OneShotDeagleConfig : ShopItemConfig {
  public override int Price { get; init; } = 100;
  public bool DoesFriendlyFire { get; init; } = true;
  public string Weapon { get; init; } = "revolver";
}