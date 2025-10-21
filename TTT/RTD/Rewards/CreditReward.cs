using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Player;
using TTT.RTD.lang;
using TTT.Shop;

namespace TTT.RTD.Rewards;

public class CreditReward(IServiceProvider provider, int amo)
  : RoundStartReward(provider) {
  public override string Name => Locale[RtdMsgs.CREDITS_REWARD(amo)];

  public override string Description
    => Locale[RtdMsgs.CREDITS_REWARD_DESC(amo)];

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public override void GiveOnRound(IOnlinePlayer player) {
    shop.AddBalance(player, amo, "RTD Reward");
  }
}