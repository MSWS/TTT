using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2.Utils;

public class DamageDealingHelper {
  public static void DealDamage(CCSPlayerController target,
    CCSPlayerController? attacker, int damage, string source,
    DamageTypes_t type = DamageTypes_t.DMG_BLAST_SURFACE) {
    if (target.Pawn.Value == null) return;

    var infoSize = Schema.GetClassSize("CTakeDamageInfo");
    var infoPtr  = Marshal.AllocHGlobal(infoSize);

    for (var i = 0; i < infoSize; i++) Marshal.WriteByte(infoPtr, i, 0);

    var damageInfo = new CTakeDamageInfo(infoPtr);

    Schema.SetSchemaValue(damageInfo.Handle, "CTakeDamageInfo", "m_hInflictor",
      attacker != null ? attacker.Pawn.Raw : 0);
    Schema.SetSchemaValue(damageInfo.Handle, "CTakeDamageInfo", "m_hAttacker",
      attacker != null ? attacker.EntityHandle.Raw : 0);
    damageInfo.Damage         = damage;
    damageInfo.BitsDamageType = type;

    if (target.Pawn.Value?.AbsOrigin != null)
      Schema.SetSchemaValue(damageInfo.Handle, "CTakeDamageInfo",
        "m_vecDamagePosition",
        target.Pawn.Value != null ?
          target.Pawn.Value.AbsOrigin.Handle :
          Vector.Zero.Handle);

    Schema.SetSchemaValue(damageInfo.Handle, "CTakeDamageInfo",
      "m_vecDamageForce", Vector.Zero.Handle);

    var damageResultSize = Schema.GetClassSize("CTakeDamageResult");
    var damageResultPtr  = Marshal.AllocHGlobal(damageResultSize);
    for (var i = 0; i < damageResultSize; i++)
      Marshal.WriteByte(damageResultPtr, i, 0);

    var damageResult = new CTakeDamageResult(damageResultPtr);
    Schema.SetSchemaValue(damageResult.Handle, "CTakeDamageResult",
      "m_pOriginatingInfo", damageInfo.Handle);

    damageResult.HealthLost          = damage;
    damageResult.DamageDealt         = damage;
    damageResult.TotalledHealthLost  = damage;
    damageResult.TotalledDamageDealt = damage;
    damageResult.WasDamageSuppressed = false;

    if (target.EntityHandle.Value != null)
      VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Invoke(
        target.EntityHandle.Value, damageInfo, damageResult);

    Marshal.FreeHGlobal(infoPtr);
    Marshal.FreeHGlobal(damageResultPtr);
  }
}

[StructLayout(LayoutKind.Explicit)]
public struct CAttackerInfo {
  [FieldOffset(0x0)]
  public bool NeedInit;

  [FieldOffset(0x1)]
  public bool IsPawn;

  [FieldOffset(0x2)]
  public bool IsWorld;

  [FieldOffset(0x4)]
  public UInt32 AttackerPawn;

  [FieldOffset(0x8)]
  public ushort AttackerUserId;

  [FieldOffset(0x0C)]
  public int TeamChecked;

  [FieldOffset(0x10)]
  public int TeamNum;
}