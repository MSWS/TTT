using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Player;

namespace TTT.RTD.Rewards;

public class CreditReward(IServiceProvider provider, int amo)
  : RoundStartReward(provider) {
  public override string Name => $"{amo} Credits";

  public override string Description
    => $"you will receive {amo} {(amo == 1 ? "credit" : "credits")} next round";

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public override void GiveOnRound(IOnlinePlayer player) {
    shop.AddBalance(player, amo, "RTD Reward");
  }
}