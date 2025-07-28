using System.Drawing;

namespace TTT.Api;

/// <summary>
/// Represents a role that can be assigned to a player.
/// Does not necessarily imply they are in the game.
/// </summary>
public interface IRole {
  string Id { get; }
  string Name { get; }
  Color Color { get; }

  bool ConflictsWith(IRole other) => false;
  bool RequiresRole(IRole other) => false;

  T? FindPlayerToAssign<T>(T player);
}