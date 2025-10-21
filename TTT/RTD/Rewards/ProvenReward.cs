using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Roles;

namespace TTT.RTD.Rewards;

public class ProvenReward(IServiceProvider provider)
  : RoundStartReward(provider) {
  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IIconManager icons =
    provider.GetRequiredService<IIconManager>();

  public override string Name => "Proven If Inno";

  public override string Description
    => "you will be shown innocent if assigned next round";

  public override void GiveOnRound(IOnlinePlayer player) {
    if (!roles.GetRoles(player).OfType<InnocentRole>().Any()) return;
    icons.RevealToAll(player);
  }
}