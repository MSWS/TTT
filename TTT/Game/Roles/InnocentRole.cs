using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.Game.lang;
using TTT.Locale;

namespace TTT.Game.Roles;

public class InnocentRole(IServiceProvider provider)
  : RatioBasedRole(provider) {
  public const string ID = "basegame.role.innocent";

  private readonly IMsgLocalizer? localizer =
    provider.GetService<IMsgLocalizer>();

  public override string Id => ID;

  public override string Name
    => localizer?[GameMsgs.ROLE_INNOCENT] ?? nameof(InnocentRole);

  public override Color Color => Color.LimeGreen;

  override protected Func<int, int> TargetCount
    => Config.BalanceCfg.InnocentCount;

  public override void OnAssign(IOnlinePlayer player) {
    base.OnAssign(player);
    var balanceConfig = Config.RoleCfg;
    player.Health = balanceConfig.InnocentHealth;
    player.Armor  = balanceConfig.InnocentArmor;

    if (balanceConfig.InnocentWeapons == null) return;

    foreach (var weapon in balanceConfig.InnocentWeapons)
      Inventory.GiveWeapon(player, new BaseWeapon(weapon));
  }
}