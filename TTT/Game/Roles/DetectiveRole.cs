using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Game.Roles;

public class DetectiveRole(IServiceProvider provider)
  : RatioBasedRole(provider, p => (int)Math.Floor(p / 8f)) {
  public const string ID = "basegame.role.detective";

  private readonly IMsgLocalizer? localizer =
    provider.GetService<IMsgLocalizer>();

  public override string Id => ID;

  public override string Name
    => localizer?[GameMsgs.ROLE_DETECTIVE] ?? nameof(DetectiveRole);

  public override Color Color => Color.DodgerBlue;

  public override void OnAssign(IOnlinePlayer player) {
    base.OnAssign(player);
    var balanceConfig = Config.BalanceCfg;
    player.Health = balanceConfig.DetectiveHealth;
    player.Armor  = balanceConfig.DetectiveArmor;

    if (balanceConfig.DetectiveWeapons == null) return;

    player.RemoveAllWeapons();
    foreach (var weapon in balanceConfig.DetectiveWeapons)
      player.GiveWeapon(weapon);
  }
}