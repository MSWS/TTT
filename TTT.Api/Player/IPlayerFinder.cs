namespace TTT.Api.Player;

public interface IPlayerFinder {
  internal void addPlayer(IOnlinePlayer player);
  internal void removePlayer(IOnlinePlayer player);

  ISet<IOnlinePlayer> GetAllPlayers();

  IOnlinePlayer? GetPlayerById(string id) {
    return string.IsNullOrEmpty(id) ?
      null :
      GetAllPlayers().FirstOrDefault(p => p.Id == id);
  }

  IOnlinePlayer? GetPlayerByName(string name) {
    var matches = GetAllPlayers().Where(p => p.Name.Contains(name)).ToList();
    return matches.Count == 1 ? matches[0] : null;
  }
}