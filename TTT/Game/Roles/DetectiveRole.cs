using System.Drawing;
using TTT.API.Player;

namespace TTT.Game.Roles;

public class DetectiveRole(IServiceProvider provider)
  : RatioBasedRole(provider) {
  public const string ID = "basegame.role.detective";

  public override string Id => ID;

  public override string Name
    => Localizer?[GameMsgs.ROLE_DETECTIVE] ?? nameof(DetectiveRole);

  public override Color Color => Color.DodgerBlue;

  override protected Func<int, int> TargetCount
    => Config.BalanceCfg.DetectiveCount;

  public override void OnAssign(IOnlinePlayer player) {
    base.OnAssign(player);
    var balanceConfig = Config.RoleCfg;
    player.Health = balanceConfig.DetectiveHealth;
    player.Armor  = balanceConfig.DetectiveArmor;

    if (balanceConfig.DetectiveWeapons == null) return;

    foreach (var weapon in balanceConfig.DetectiveWeapons)
      Inventory.GiveWeapon(player, new BaseWeapon(weapon));
  }
}