using CounterStrikeSharp.API.Core;

namespace TTT.CS2.Extensions;

public static class PlayerExtensions {
  public static CBasePlayerWeapon? GetWeaponBase(
    this CCSPlayerController player, string designerName) {
    if (!player.IsValid) return null;
    var pawn = player.PlayerPawn.Value;
    if (pawn == null || !pawn.IsValid) return null;

    return pawn.WeaponServices?.MyWeapons
     .FirstOrDefault(w => w.Value?.DesignerName == designerName)
    ?.Value;
  }
}