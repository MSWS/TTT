using System.Drawing;
using TTT.API.Player;

namespace TTT.API.Role;

/// <summary>
///   Represents a role that can be assigned to a player.
///   Does not necessarily imply they are in the game.
/// </summary>
public interface IRole : IEquatable<IRole> {
  string Id { get; }
  string Name { get; }
  Color Color { get; }

  bool IEquatable<IRole>.Equals(IRole? other) {
    if (other is null) return false;
    if (ReferenceEquals(this, other)) return true;
    return Id == other.Id;
  }

  IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players);

  void OnAssign(IOnlinePlayer player) { }
}