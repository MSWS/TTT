using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;

namespace TTT.CS2.Extensions;

public static class PlayerExtensions {
  public enum FadeFlags {
    FADE_IN, FADE_OUT, FADE_STAYOUT
  }

  public static CBasePlayerWeapon? GetWeaponBase(
    this CCSPlayerController player, string designerName) {
    if (!player.IsValid) return null;
    var pawn = player.Pawn.Value;
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

  public static void SetHealth(this CCSPlayerController player, int health) {
    if (player.Pawn.Value == null) return;
    if (health <= 0) {
      player.CommitSuicide(false, true);
      return;
    }

    player.Pawn.Value.Health = health;
    Utilities.SetStateChanged(player.Pawn.Value, "CBaseEntity", "m_iHealth");
  }

  public static int GetHealth(this CCSPlayerController player) {
    return player.Pawn.Value?.Health ?? 0;
  }

  public static void AddHealth(this CCSPlayerController player, int health) {
    if (player.Pawn.Value == null) return;
    player.SetHealth(player.Pawn.Value.Health + health);
  }

  public static void SetColor(this CCSPlayerController player, Color color) {
    if (!player.IsValid) return;
    var pawn = player.Pawn.Value;
    if (!player.IsValid || pawn == null || !pawn.IsValid) return;

    if (color.A == 255)
      color = Color.FromArgb(pawn.Render.A == 255 ? 255 : 254, color.R, color.G,
        color.B);
    pawn.SetColor(color);
  }

  public static void ColorScreen(this CCSPlayerController player, Color color,
    float hold = 0.1f, float fade = 0.2f, FadeFlags flags = FadeFlags.FADE_IN,
    bool withPurge = true) {
    var fadeMsg = UserMessage.FromId(106);

    fadeMsg.SetInt("duration", Convert.ToInt32(fade * 512));
    fadeMsg.SetInt("hold_time", Convert.ToInt32(hold * 512));

    var flag = flags switch {
      FadeFlags.FADE_IN      => 0x0001,
      FadeFlags.FADE_OUT     => 0x0002,
      FadeFlags.FADE_STAYOUT => 0x0008,
      _                      => 0x0001
    };

    if (withPurge) flag |= 0x0010;

    fadeMsg.SetInt("flags", flag);
    fadeMsg.SetInt("color",
      color.R | color.G << 8 | color.B << 16 | color.A << 24);
    fadeMsg.Send(player);
  }
}