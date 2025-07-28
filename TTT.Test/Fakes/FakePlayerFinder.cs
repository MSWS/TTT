using TTT.Api.Player;

namespace TTT.Test.Fakes;

public class FakePlayerFinder : IPlayerFinder {
  private readonly HashSet<IOnlinePlayer> players = [];

  public void addPlayer(IOnlinePlayer player) => players.Add(player);

  public void removePlayer(IOnlinePlayer player) => players.Remove(player);

  public ISet<IOnlinePlayer> GetAllPlayers() => players;
}