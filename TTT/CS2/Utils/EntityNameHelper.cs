using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;

namespace TTT.CS2.Utils;

public class EntityNameHelper {
  private static readonly CEntityIdentity_SetEntityName
    CEntityIdentity_SetEntityNameFunc;

  static EntityNameHelper() {
    var setEntityNameSignature = NativeAPI.FindSignature(Addresses.ServerPath,
      GameData.GetSignature("CEntityIdentity_SetEntityNameFunc"));
    CEntityIdentity_SetEntityNameFunc =
      Marshal.GetDelegateForFunctionPointer<CEntityIdentity_SetEntityName>(
        setEntityNameSignature);
  }

  private delegate void CEntityIdentity_SetEntityName(IntPtr ptr, string name);

  public static void SetEntityName(CEntityIdentity identity, string name) {
    CEntityIdentity_SetEntityNameFunc(identity.Handle, name);
  }
}