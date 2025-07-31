using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Game.Roles;

public class InnocentRole(IServiceProvider provider) : BaseRole(provider) {
  public const string ID = "basegame.role.innocent";
  public override string Id => ID;

  private readonly IMsgLocalizer? localizer =
    provider.GetService<IMsgLocalizer>();

  public override string Name
    => localizer?[GameMsgs.ROLE_INNOCENT] ?? nameof(InnocentRole);

  public override Color Color => Color.LimeGreen;

  public override IOnlinePlayer?
    FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    return players.FirstOrDefault(p => p.Roles.Count == 0);
  }
}