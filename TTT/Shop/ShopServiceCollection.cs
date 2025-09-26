using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;
using TTT.Shop.Commands;
using TTT.Shop.Items;
using TTT.Shop.Listeners;

namespace TTT.Shop;

public static class ShopServiceCollection {
  public static void AddShopServices(this IServiceCollection collection) {
    collection.AddModBehavior<IShop, Shop>();

    collection.AddModBehavior<RoundShopClearer>();
    collection.AddModBehavior<RoleAssignCreditor>();

    collection.AddModBehavior<ShopCommand>();
    collection.AddModBehavior<BuyCommand>();
    collection.AddModBehavior<BalanceCommand>();

    collection.AddDeagleServices();
  }
}