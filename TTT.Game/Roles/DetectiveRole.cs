using System.Drawing;

namespace TTT.Game.Roles;

public class DetectiveRole()
  : RatioBasedRole((int p) => (int)Math.Floor(p / 8f)) {
  public const string ID = "basegame.role.detective";
  public override string Id => ID;
  public override string Name => "Detective";
  public override Color Color => Color.DodgerBlue;
}