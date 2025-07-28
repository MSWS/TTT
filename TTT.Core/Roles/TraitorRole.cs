using System.Drawing;
using TTT.Api;

namespace TTT.Core.Roles;

public class TraitorRole : IRole {
  public const string ID = "core.role.traitor";
  public string Id => ID;
  public string Name => "Traitor";
  public Color Color => Color.Red;

  public IPlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    return players.Count == 0 ?
      null :
      players.ElementAt(new Random().Next(players.Count));
  }
}