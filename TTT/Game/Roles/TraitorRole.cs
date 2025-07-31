using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.Locale;

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

    player.RemoveAllWeapons();
    foreach (var weapon in balanceConfig.TraitorWeapons)
      player.GiveWeapon(weapon);
  }
}