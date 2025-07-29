using System.Drawing;

namespace TTT.Game.Roles;

public class TraitorRole()
  : RatioBasedRole(p => (int)Math.Ceiling((p - 1f) / 5f)) {
  public const string ID = "basegame.role.traitor";
  public override string Id => ID;
  public override string Name => "Traitor";
  public override Color Color => Color.Red;
}