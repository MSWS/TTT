using System.Drawing;
using TTT.Locale;

namespace TTT.Game.Roles;

public class DetectiveRole(IMsgLocalizer? localizer = null)
  : RatioBasedRole(p => (int)Math.Floor(p / 8f)) {
  public const string ID = "basegame.role.detective";
  public override string Id => ID;

  public override string Name
    => localizer?[GameMsgs.ROLE_DETECTIVE] ?? nameof(DetectiveRole);

  public override Color Color => Color.DodgerBlue;
}