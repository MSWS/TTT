using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.CS2.RayTrace.Class;
using Address = TTT.CS2.Utils.Address;

namespace TTT.CS2.Items.ClusterGrenade;

public class GrenadeDataHelper {
  private static readonly CHEGrenadeProjectile_CreateDelegate
    CHEGrenadeProjectile_CreateFunc;

  static GrenadeDataHelper() {
    var heGrenadeSignature = NativeAPI.FindSignature(Addresses.ServerPath,
      GameData.GetSignature("CHEGrenadeProjectile_CreateFunc"));
    CHEGrenadeProjectile_CreateFunc =
      Marshal
       .GetDelegateForFunctionPointer<CHEGrenadeProjectile_CreateDelegate>(
          heGrenadeSignature);
  }

  private delegate int CHEGrenadeProjectile_CreateDelegate(IntPtr position,
    IntPtr angle, IntPtr velocity, IntPtr velocityAngle, IntPtr thrower,
    int weaponId, byte team);

  public static int CreateGrenade(Vector position, QAngle angle,
    Vector velocity, Vector velocityAngle, IntPtr thrower, CsTeam team) {
    return CHEGrenadeProjectile_CreateFunc(position.Handle, angle.Handle,
      velocity.Handle, velocityAngle.Handle, thrower, 44, (byte)team);
  }
}