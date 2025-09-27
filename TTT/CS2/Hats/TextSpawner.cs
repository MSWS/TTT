using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.CS2.Extensions;
using TTT.CS2.RayTrace.Class;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2.Hats;

public class TextSpawner : ITextSpawner {
  public CPointWorldText CreateText(TextSetting setting, Vector position,
    QAngle rot) {
    var ent = Utilities.CreateEntityByName<CPointWorldText>("point_worldtext");

    if (ent == null || !ent.IsValid)
      throw new Exception("Failed to create CPointWorldText entity");

    ent.MessageText     = setting.msg;
    ent.Enabled         = setting.enabled;
    ent.FontSize        = setting.fontSize;
    ent.Color           = setting.color;
    ent.WorldUnitsPerPx = setting.worldUnitsPerPx;
    ent.DepthOffset     = setting.depthOffset;
    ent.Fullbright      = setting.fullbright;
    ent.FontName        = setting.fontName;

    ent.Teleport(position, rot);
    ent.DispatchSpawn();
    return ent;
  }

  public IEnumerable<CPointWorldText> CreateTextHat(TextSetting setting,
    CCSPlayerController player) {
    var one = spawnHatPart(setting, player, 270);
    var two = spawnHatPart(setting, player, 180);

    return [one, two];
  }

  public IEnumerable<CPointWorldText> CreateTextScreen(TextSetting setting,
    CCSPlayerController player) {
    var screen = spawnScreen(setting, player);
    return [screen];
  }

  private static readonly QAngle screenAngle = new(90, 180, 0);

  private CPointWorldText spawnScreen(TextSetting setting,
    CCSPlayerController player) {
    if (player.Pawn.Value == null || player.Pawn.Value.AbsRotation == null)
      throw new Exception("Failed to get player rotation");
    var eyes       = player.GetEyePosition().Clone()!;
    var forward    = player.Pawn.Value.AbsRotation.Clone()!.ToForward();
    var inFront    = eyes + forward * 50;
    var localAngle = player.Pawn.Value.AbsRotation.Clone()!;
    localAngle = new QAngle(screenAngle.X, localAngle.Y + screenAngle.Y,
      screenAngle.Z);

    var ent = CreateText(setting, inFront, screenAngle);
    ent.AcceptInput("SetParent", player.Pawn.Value, null, "!activator");
    return ent;
  }

  private CPointWorldText spawnHatPart(TextSetting setting,
    CCSPlayerController player, float yRot) {
    var position = player.Pawn.Value?.AbsOrigin;
    var rotation = player.Pawn.Value?.AbsRotation;
    if (position == null || rotation == null)
      throw new Exception("Failed to get player position");
    position = position.Clone()!;
    position.Add(new Vector(0, 0, 72));
    rotation = new QAngle(rotation.X, rotation.Y + yRot, rotation.Z + 90);

    position.Add(GetRightVector(rotation) * -10);

    var ent = CreateText(setting, position, rotation);
    ent.AcceptInput("SetParent", player.Pawn.Value, null, "!activator");
    return ent;
  }

  public static Vector GetRightVector(QAngle rotation) {
    var forward = new Vector {
      X = (float)Math.Cos(rotation.Y * Math.PI / 180),
      Y = (float)Math.Sin(rotation.Y * Math.PI / 180),
      Z = 0
    };
    return forward;
  }
}