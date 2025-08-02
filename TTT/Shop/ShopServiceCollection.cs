using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;

namespace TTT.Shop;

public static class ShopServiceCollection {
  public static void AddShopServices(this IServiceCollection collection) {
    collection.AddModBehavior<IShop, Shop>();
  }
}