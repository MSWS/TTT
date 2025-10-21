using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.CS2.Utils;
using TTT.Game.Roles;

namespace TTT.RTD.Rewards;

public class WeaponReward(IServiceProvider provider, string weapon,
  int? amo = null, int? reserve = null) : RoundStartReward(provider) {
  private readonly IInventoryManager inventory =
    provider.GetRequiredService<IInventoryManager>();

  public override string Name
    => WeaponTranslations.GetFriendlyWeaponName(weapon);

  public override string Description
    => $"you will receive {(weapon.ToLower().StartsWith('a') ? "an" : "a")} {WeaponTranslations.GetFriendlyWeaponName(weapon)} next round{ammoDesc}";

  private string ammoDesc
    => amo != null && reserve != null ? $" with {amo}/{reserve} ammo" :
      amo != null ? $" with {amo} ammo" :
      reserve != null ? $" with {reserve} reserve ammo" : "";

  public override void GiveOnRound(IOnlinePlayer player) {
    inventory.GiveWeapon(player, new BaseWeapon(weapon, amo, reserve));
  }
}