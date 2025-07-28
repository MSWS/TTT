using System.Drawing;

namespace TTT.Game.Roles;

public class DetectiveRole(float targetRatio = 1f / 8f)
  : RatioBasedRole(targetRatio) {
  public const string ID = "basegame.role.detective";
  public override string Id => ID;
  public override string Name => "Detective";
  public override Color Color => Color.DodgerBlue;
}