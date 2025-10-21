using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Player;

namespace TTT.RTD.Rewards;

public class ShopItemReward<TItem>(IServiceProvider provider)
  : RoundStartReward(provider) where TItem : IShopItem {
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public override string Name => typeof(TItem).Name;

  public override string Description
    => $"you will receive {("aeiou".Contains(Name.ToLower()[0]) ? "an" : "a")} {typeof(TItem).Name} next round";

  public override void GiveOnRound(IOnlinePlayer player) {
    var instance = shop.Items.OfType<TItem>().FirstOrDefault();
    if (instance == null) return;
    shop.GiveItem(player, instance);
  }
}