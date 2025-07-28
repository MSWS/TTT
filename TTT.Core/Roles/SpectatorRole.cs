using System.Drawing;
using TTT.Api;

namespace TTT.Core.Roles;

public class SpectatorRole : IRole {
  public string Id => "core.role.spectator";
  public string Name => "Spectator";
  public Color Color => Color.Gray;

  public T? FindPlayerToAssign<T>(T player) => default;
}