using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Game.Roles;

public class DetectiveRole(IServiceProvider provider)
  : RatioBasedRole(provider, p => (int)Math.Floor(p / 8f)) {
  public const string ID = "basegame.role.detective";
  public override string Id => ID;

  private readonly IMsgLocalizer? localizer =
    provider.GetService<IMsgLocalizer>();

  public override string Name
    => localizer?[GameMsgs.ROLE_DETECTIVE] ?? nameof(DetectiveRole);

  public override Color Color => Color.DodgerBlue;
}