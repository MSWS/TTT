using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Locale;

namespace TTT.Shop.Items;

public abstract class BaseItem(IServiceProvider provider) : IShopItem {
  protected readonly IServiceProvider Provider = provider;
  protected readonly IShop Shop = provider.GetRequiredService<IShop>();

  protected readonly IRoleAssigner Roles =
    provider.GetRequiredService<IRoleAssigner>();

  protected readonly IMsgLocalizer Locale =
    provider.GetRequiredService<IMsgLocalizer>();

  protected readonly IInventoryManager Inventory =
    provider.GetRequiredService<IInventoryManager>();

  public void Dispose() { }
  public abstract string Name { get; }
  public abstract string Description { get; }
  public abstract ShopItemConfig Config { get; }
  public abstract void OnPurchase(IOnlinePlayer player);
  public abstract PurchaseResult CanPurchase(IOnlinePlayer player);

  public void Start() { Shop.RegisterItem(this); }
}