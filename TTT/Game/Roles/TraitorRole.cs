using System.Drawing;
using TTT.Locale;

namespace TTT.Game.Roles;

public class TraitorRole(IMsgLocalizer? localizer = null)
  : RatioBasedRole(p => (int)Math.Ceiling((p - 1f) / 5f)) {
  public const string ID = "basegame.role.traitor";
  public override string Id => ID;

  public override string Name
    => localizer?[GameMsgs.ROLE_TRAITOR] ?? nameof(TraitorRole);

  public override Color Color => Color.Red;
}