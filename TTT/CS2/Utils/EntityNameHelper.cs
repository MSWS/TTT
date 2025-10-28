using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace TTT.CS2.Utils;

public class EntityNameHelper {
  private static readonly CEntityIdentity_SetEntityName
    CEntityIdentity_SetEntityNameFunc;
  // private static MemoryFunctionVoid<CEntityIdentity, string>
  //   CEntityIdentity_SetEntityNameFunc;

  static EntityNameHelper() {
    var setEntityNameSignature = NativeAPI.FindSignature(Addresses.ServerPath,
      GameData.GetSignature("CEntityIdentity_SetEntityNameFunc"));
    CEntityIdentity_SetEntityNameFunc =
      Marshal.GetDelegateForFunctionPointer<CEntityIdentity_SetEntityName>(
        setEntityNameSignature);
  }

  private delegate void CEntityIdentity_SetEntityName(IntPtr ptr, string name);

  public static void SetEntityName(CCSPlayerController player, string name) {
    if (player.Pawn.Value == null) return;
    if (player.Pawn.Value.Entity != null)
      CEntityIdentity_SetEntityNameFunc(player.Pawn.Value.Entity.Handle, name);
  }
}