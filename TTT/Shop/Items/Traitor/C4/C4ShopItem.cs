using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API.Events;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Game.Roles;

namespace TTT.Shop.Items.Traitor.C4;

public static class C4ServiceCollection {
  public static void AddC4Services(this IServiceCollection collection) {
    collection.AddModBehavior<C4ShopItem>();
  }
}

public class C4ShopItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider), IListener {
  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private int c4sBought;

  private C4Config config
    => Provider.GetService<IStorage<C4Config>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new C4Config();

  public override string Name => Locale[C4Msgs.SHOP_ITEM_C4];
  public override string Description => Locale[C4Msgs.SHOP_ITEM_C4_DESC];

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) {
    c4sBought++;
    Inventory.GiveWeapon(player, new BaseWeapon(config.Weapon));
  }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    if (config.MaxC4PerRound > 0)
      if (c4sBought > config.MaxC4PerRound)
        return PurchaseResult.ITEM_NOT_PURCHASABLE;

    if (config.MaxC4AtOnce > 0)
      if (finder.GetOnline().Count(p => Shop.HasItem<C4ShopItem>(p))
        > config.MaxC4AtOnce)
        return PurchaseResult.ITEM_NOT_PURCHASABLE;

    return base.CanPurchase(player);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;
    c4sBought = 0;
  }
}