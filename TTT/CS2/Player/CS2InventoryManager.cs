using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.API;
using TTT.API.Player;
using TTT.CS2.Extensions;

namespace TTT.CS2.Player;

public class CS2InventoryManager(
  IPlayerConverter<CCSPlayerController> converter) : IInventoryManager {
  public Task GiveWeapon(IOnlinePlayer player, IWeapon weapon) {
    return Server.NextWorldUpdateAsync(() => {
      var gamePlayer = converter.GetPlayer(player);
      if (gamePlayer == null) return;

      giveWeapon(gamePlayer, weapon);
    });
  }

  public Task RemoveWeapon(IOnlinePlayer player, string weaponId) {
    return Server.NextWorldUpdateAsync(() => {
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
    });
  }

  public Task RemoveWeaponInSlot(IOnlinePlayer player, int slot) {
    return Server.NextWorldUpdateAsync(() => {
      if (!player.IsAlive) return;

      var gamePlayer = converter.GetPlayer(player);
      if (gamePlayer == null) return;

      clearSlot(gamePlayer, IntToSlot(slot));
    });
  }

  public Task RemoveAllWeapons(IOnlinePlayer player) {
    return Server.NextWorldUpdateAsync(() => {
      if (!player.IsAlive) return;

      var gamePlayer = converter.GetPlayer(player);
      if (gamePlayer == null) return;

      gamePlayer.RemoveWeapons();
    });
  }

  private void giveWeapon(CCSPlayerController player, IWeapon weapon) {
    if (player.Team is CsTeam.None or CsTeam.Spectator) return;

    // Give the weapon
    player.GiveNamedItem(weapon.WeaponId);

    // Set ammo if applicable
    var weaponBase = player.GetWeaponBase(weapon.WeaponId);
    if (weaponBase == null)
      if (weapon.WeaponId.Equals("weapon_revolver"))
        weaponBase = player.GetWeaponBase("weapon_deagle");

    if (weaponBase == null) return;

    if (weapon.CurrentAmmo != null) weaponBase.Clip1 = weapon.CurrentAmmo.Value;
    if (weapon.ReserveAmmo != null)
      weaponBase.ReserveAmmo[0] = weapon.ReserveAmmo.Value;
    Utilities.SetStateChanged(weaponBase, "CBasePlayerWeapon", "m_iClip1");
    Utilities.SetStateChanged(weaponBase, "CBasePlayerWeapon",
      "m_pReserveAmmo");
  }

  public static gear_slot_t IntToSlot(int slot) {
    return slot switch {
      0 => gear_slot_t.GEAR_SLOT_RIFLE,
      1 => gear_slot_t.GEAR_SLOT_PISTOL,
      2 => gear_slot_t.GEAR_SLOT_KNIFE,
      3 => gear_slot_t.GEAR_SLOT_UTILITY,
      4 => gear_slot_t.GEAR_SLOT_C4,
      _ => gear_slot_t.GEAR_SLOT_FIRST
    };
  }

  public static int SlotToInt(gear_slot_t slot) {
    return slot switch {
      gear_slot_t.GEAR_SLOT_RIFLE   => 0,
      gear_slot_t.GEAR_SLOT_PISTOL  => 1,
      gear_slot_t.GEAR_SLOT_KNIFE   => 2,
      gear_slot_t.GEAR_SLOT_UTILITY => 3,
      gear_slot_t.GEAR_SLOT_C4      => 4,
      _                             => -1
    };
  }

  private void clearSlot(CCSPlayerController player,
    params gear_slot_t[] slots) {
    if (player.Team is CsTeam.None or CsTeam.Spectator) return;
    var weapons = player.Pawn.Value?.WeaponServices?.MyWeapons;
    if (weapons == null || weapons.Count == 0) return;

    foreach (var weapon in weapons) {
      if (!weapon.IsValid || weapon.Value == null) continue;
      if (!weapon.Value.IsValid
        || !weapon.Value.DesignerName.StartsWith("weapon_"))
        continue;
      if (weapon.Value.Entity == null) continue;
      var weaponBase = weapon.Value.As<CCSWeaponBase>();
      if (!weaponBase.IsValid || weaponBase.Entity == null) continue;
      var vdata = weaponBase.VData;
      if (vdata == null) continue;
      if (!slots.Contains(vdata.GearSlot)) continue;
      weapon.Value.AddEntityIOEvent("Kill", weapon.Value);
    }
  }
}