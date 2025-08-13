using System.Drawing;
using TTT.API.Player;

namespace TTT.Game.Roles;

public class TraitorRole(IServiceProvider provider) : RatioBasedRole(provider) {
  public const string ID = "basegame.role.traitor";
  public override string Id => ID;

  public override string Name
    => Localizer?[GameMsgs.ROLE_TRAITOR] ?? nameof(TraitorRole);

  public override Color Color => Color.Red;

  override protected Func<int, int> TargetCount
    => Config.BalanceCfg.TraitorCount;

  public override void OnAssign(IOnlinePlayer player) {
    base.OnAssign(player);
    var balanceConfig = Config.RoleCfg;
    player.Health = balanceConfig.TraitorHealth;
    player.Armor  = balanceConfig.TraitorArmor;

    if (balanceConfig.TraitorWeapons == null) return;

    foreach (var weapon in balanceConfig.TraitorWeapons)
      Inventory.GiveWeapon(player, new BaseWeapon(weapon));
  }
}