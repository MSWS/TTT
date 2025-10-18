using System.Net;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;

namespace TTT.CS2.Utils;

public class DamageDealingHelper {
  public static void DealDamage(CCSPlayerController target,
    CCSPlayerController? attacker, int damage, string source,
    DamageTypes_t type = DamageTypes_t.DMG_BULLET) {
    var infoSize = Schema.GetClassSize("CTakeDamageInfo");
    var infoPtr  = Marshal.AllocHGlobal(infoSize);

    for (var i = 0; i < infoSize; i++) Marshal.WriteByte(infoPtr, i, 0);

    var damageInfo = new CTakeDamageInfo(infoPtr);

    var attackerInfo = new CAttackerInfo() {
      AttackerUserId =
        attacker != null && attacker.UserId != null ?
          (ushort)attacker.UserId :
          unchecked((ushort)(-1)),
      AttackerPawn = attacker != null ? attacker.Pawn.Raw : 0,
      NeedInit     = false,
      TeamNum =
        attacker != null && attacker.Pawn.Value != null ?
          attacker.Pawn.Value.TeamNum :
          0,
      TeamChecked = 0,
      IsWorld     = attacker == null,
      IsPawn      = attacker != null
    };

    Marshal.StructureToPtr(attackerInfo, new IntPtr(infoPtr.ToInt64() + 0x80),
      false);

    Schema.SetSchemaValue(damageInfo.Handle, "CTakeDamageInfo", "m_hInflictor",
      attacker != null ? attacker.Pawn.Raw : 0);
    Schema.SetSchemaValue(damageInfo.Handle, "CTakeDamageInfo", "m_hAttacker",
      attacker != null ? attacker.EntityHandle.Raw : 0);
    damageInfo.Damage         = damage;
    damageInfo.BitsDamageType = type;

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

    if (target.Pawn.Value != null)
      VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Invoke(target.Pawn.Value,
        damageInfo, damageResult);
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