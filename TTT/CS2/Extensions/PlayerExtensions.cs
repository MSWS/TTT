using System.Drawing;
using CounterStrikeSharp.API;
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

  public static void SetClan(this CCSPlayerController player, string clan,
    bool flush = true) {
    player.Clan = clan;
    Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
    if (!flush) return;
    var ev = new EventNextlevelChanged(true);
    ev.FireEvent(false);
  }

  public static void SetColor(this CCSPlayerController player, Color color) {
    if (!player.IsValid) return;
    var pawn = player.PlayerPawn.Value;
    if (!player.IsValid || pawn == null || !pawn.IsValid) return;

    if (color.A == 255)
      color = Color.FromArgb(pawn.Render.A, color.R, color.G, color.B);
    player.PlayerPawn.Value.SetColor(color);
  }
}