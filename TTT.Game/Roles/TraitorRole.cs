using System.Drawing;
using TTT.Api;
using TTT.Api.Player;

namespace TTT.Game.Roles;

public class TraitorRole(float targetRatio = 1f / 5f)
  : RatioBasedRole(targetRatio) {
  public const string ID = "basegame.role.traitor";
  public override string Id => ID;
  public override string Name => "Traitor";
  public override Color Color => Color.Red;
}