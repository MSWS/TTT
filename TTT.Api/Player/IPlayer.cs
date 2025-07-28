namespace TTT.Api.Player;

public interface IPlayer {
  /// <summary>
  /// The unique identifier for the player, should
  /// be unique across all players at all times.
  /// </summary>
  string Id { get; }

  string Name { get; }
}