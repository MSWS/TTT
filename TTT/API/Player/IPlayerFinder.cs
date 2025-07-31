namespace TTT.API.Player;

public interface IPlayerFinder {
  public IOnlinePlayer AddPlayer(IOnlinePlayer player);
  public IPlayer RemovePlayer(IPlayer player);

  ISet<IOnlinePlayer> GetOnline();

  IOnlinePlayer? GetPlayerById(string id) {
    return string.IsNullOrEmpty(id) ?
      null :
      GetOnline().FirstOrDefault(p => p.Id == id);
  }

  IOnlinePlayer? GetPlayerByName(string name) {
    var matches = GetOnline().Where(p => p.Name.Contains(name)).ToList();
    return matches.Count == 1 ? matches[0] : null;
  }
}