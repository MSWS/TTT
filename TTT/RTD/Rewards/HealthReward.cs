using TTT.API.Player;

namespace TTT.RTD.Rewards;

public class HealthReward(IServiceProvider provider, int health)
  : RoundStartReward(provider) {
  public override string Name => $"{health} HP";

  public override string Description
    => $"you will start next round with {health} HP";

  public override void GiveOnRound(IOnlinePlayer player) {
    player.Health = health;
  }
}