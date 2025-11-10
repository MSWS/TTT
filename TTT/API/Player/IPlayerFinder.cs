namespace TTT.API.Player;

public interface IPlayerFinder {
  public IOnlinePlayer AddPlayer(IOnlinePlayer player);

  public void AddPlayers(params IOnlinePlayer[] players) {
    foreach (var p in players) AddPlayer(p);
  }

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

  List<IOnlinePlayer> GetMulti(string query, out string name,
    IOnlinePlayer? executor = null) {
    var result = query switch {
      "@all" => GetOnline().ToList(),
      "@me"  => executor != null ? new List<IOnlinePlayer> { executor } : [],
      "@!me" => executor != null ?
        GetOnline().Where(p => p.Id != executor.Id).ToList() :
        GetOnline().ToList(),
      _ => GetSingle(query) != null ?
        new List<IOnlinePlayer> { GetSingle(query)! } : []
    };

    name = "no players found";
    name = query switch {
      "@all" => "all players",
      "@me"  => executor != null ? executor.Name : "no one",
      "@!me" => executor != null ?
        $"all players except {executor.Name}" :
        "all players",
      _ => GetSingle(query) != null ?
        GetSingle(query)!.Name :
        "no players found"
    };

    return result;
  }

  IOnlinePlayer? GetSingle(string query) {
    if (query.StartsWith("#")) {
      var id   = query[1..];
      var byId = GetPlayerById(id);
      if (byId != null) return byId;
      var byName = GetOnline().FirstOrDefault(p => p.Name == id);
      return byName;
    }

    var byNameExact = GetOnline().FirstOrDefault(p => p.Name == query);
    if (byNameExact != null) return byNameExact;

    var contains = GetOnline().Where(p => p.Name.Contains(query)).ToList();

    if (contains.Count == 1) return contains[0];

    contains = GetOnline()
     .Where(p
        => p.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase))
     .ToList();

    return contains.Count == 1 ? contains[0] : null;
  }
}