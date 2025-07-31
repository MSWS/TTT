using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using TTT.Locale;

namespace TTT.Game.Roles;

public class TraitorRole(IServiceProvider provider)
  : RatioBasedRole(provider, p => (int)Math.Ceiling((p - 1f) / 5f)) {
  public const string ID = "basegame.role.traitor";
  public override string Id => ID;

  private readonly IMsgLocalizer? localizer =
    provider.GetService<IMsgLocalizer>();

  public override string Name
    => localizer?[GameMsgs.ROLE_TRAITOR] ?? nameof(TraitorRole);

  public override Color Color => Color.Red;
}