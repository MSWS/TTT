using CounterStrikeSharp.API.Core;
using TTT.API;
using TTT.API.Player;
using TTT.CS2.Extensions;

namespace TTT.CS2;

public class CS2InventoryManager(
  IPlayerConverter<CCSPlayerController> converter) : IInventoryManager {
  public void GiveWeapon(IOnlinePlayer player, IWeapon weapon) {
    var gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return;

    gamePlayer.GiveNamedItem(weapon.Id);
    if (weapon.ReserveAmmo == null && weapon.CurrentAmmo == null) return;
    gamePlayer.GetWeaponBase(weapon.Id);
  }

  public void RemoveWeapon(IOnlinePlayer player, string weaponId) {
    if (!player.IsAlive) return;

    var gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return;

    var pawn = gamePlayer.PlayerPawn.Value;

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

    // TODO: Verify 1f is required here (and not 0.1 or similar)
    activeWeaponEntity?.AddEntityIOEvent("Kill", activeWeaponEntity, null, "",
      1f);
  }

  public void RemoveAllWeapons(IOnlinePlayer player) {
    if (!player.IsAlive) return;

    var gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return;

    gamePlayer.RemoveWeapons();
  }
}