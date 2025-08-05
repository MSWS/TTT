using CounterStrikeSharp.API;
using TTT.API.Role;
using TTT.CS2.Roles;
using TTT.Game;

namespace TTT.CS2;

public class CS2Game(IServiceProvider provider) : RoundBasedGame(provider) {
  public override IList<IRole> Roles { get; } = [
    new SpectatorRole(provider), new CS2InnocentRole(provider),
    new CS2TraitorRole(provider), new CS2DetectiveRole(provider)
  ];

  override protected void StartRound() {
    Server.NextWorldUpdate(() => { base.StartRound(); });
  }
}