using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Configs;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Locale;

namespace ShopAPI;

public abstract class BaseItem(IServiceProvider provider) : IShopItem {
  protected readonly IPlayerFinder Finder =
    provider.GetRequiredService<IPlayerFinder>();

  protected readonly IGameManager Games =
    provider.GetRequiredService<IGameManager>();

  protected readonly IInventoryManager Inventory =
    provider.GetRequiredService<IInventoryManager>();

  protected readonly IMsgLocalizer Locale =
    provider.GetRequiredService<IMsgLocalizer>();

  protected readonly IMessenger Messenger =
    provider.GetRequiredService<IMessenger>();

  protected readonly IServiceProvider Provider = provider;

  protected readonly IRoleAssigner Roles =
    provider.GetRequiredService<IRoleAssigner>();

  protected readonly IShop Shop = provider.GetRequiredService<IShop>();

  public virtual void Dispose() { }
  public abstract string Name { get; }
  public abstract string Description { get; }
  public abstract ShopItemConfig Config { get; }
  public abstract void OnPurchase(IOnlinePlayer player);
  public abstract PurchaseResult CanPurchase(IOnlinePlayer player);

  public virtual void Start() { Shop.RegisterItem(this); }
}