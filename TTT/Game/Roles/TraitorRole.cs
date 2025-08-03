using System.Drawing;
using TTT.API.Player;

namespace TTT.Game.Roles;

public class TraitorRole(IServiceProvider provider)
  : RatioBasedRole(provider, p => (int)Math.Ceiling((p - 1f) / 5f)) {
  public const string ID = "basegame.role.traitor";
  public override string Id => ID;

  public override string Name
    => Localizer?[GameMsgs.ROLE_TRAITOR] ?? nameof(TraitorRole);

  public override Color Color => Color.Red;

  public override void OnAssign(IOnlinePlayer player) {
    base.OnAssign(player);
    var balanceConfig = Config.BalanceCfg;
    player.Health = balanceConfig.TraitorHealth;
    player.Armor  = balanceConfig.TraitorArmor;

    if (balanceConfig.TraitorWeapons == null) return;

    Inventory.RemoveAllWeapons(player);
    foreach (var weapon in balanceConfig.TraitorWeapons)
      Inventory.GiveWeapon(player, new BaseWeapon(weapon));
  }
}