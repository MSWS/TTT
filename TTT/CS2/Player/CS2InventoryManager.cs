using CounterStrikeSharp.API.Core;
using TTT.API;
using TTT.API.Player;
using TTT.CS2.Extensions;

namespace TTT.CS2.Player;

public class CS2InventoryManager(
  IPlayerConverter<CCSPlayerController> converter) : IInventoryManager {
  public void GiveWeapon(IOnlinePlayer player, IWeapon weapon) {
    var gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return;

    gamePlayer.GiveNamedItem(weapon.Id);
    if (weapon.ReserveAmmo == null && weapon.CurrentAmmo == null) return;
    var weaponBase = gamePlayer.GetWeaponBase(weapon.Id);
    if (weaponBase == null) return;
    if (weapon.CurrentAmmo != null) weaponBase.Clip1 = weapon.CurrentAmmo.Value;
    if (weapon.ReserveAmmo != null) weaponBase.Clip2 = weapon.ReserveAmmo.Value;
  }

  public void RemoveWeapon(IOnlinePlayer player, string weaponId) {
    if (!player.IsAlive) return;

    var gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return;

    var pawn = gamePlayer.Pawn.Value;

    if (pawn == null || pawn.WeaponServices == null) return;

    var matchedWeapon =
      pawn.WeaponServices.MyWeapons.FirstOrDefault(x
        => x.Value?.DesignerName == weaponId);

    if (matchedWeapon?.Value == null || !matchedWeapon.IsValid) return;
    pawn.WeaponServices.ActiveWeapon.Raw = matchedWeapon.Raw;

    // Make them equip the desired weapon
    var activeWeaponEntity =
      pawn.WeaponServices.ActiveWeapon.Value?.As<CBaseEntity>();

    gamePlayer.DropActiveWeapon();
    activeWeaponEntity?.AddEntityIOEvent("Kill", activeWeaponEntity);
  }

  public void RemoveAllWeapons(IOnlinePlayer player) {
    if (!player.IsAlive) return;

    var gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return;

    gamePlayer.RemoveWeapons();
  }
}