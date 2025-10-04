namespace TTT.API.Player;

public interface IPlayer : IEquatable<IPlayer> {
  /// <summary>
  ///   The unique identifier for the player, should
  ///   be unique across all players at all times.
  /// </summary>
  string Id { get; }

  string Name { get; }

  bool IEquatable<IPlayer>.Equals(IPlayer? other) {
    if (other is null) return false;
    return Id == other.Id;
  }
}