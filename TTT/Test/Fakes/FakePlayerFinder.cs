using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.Test.Fakes;

public class FakePlayerFinder(IEventBus bus) : IPlayerFinder {
  private readonly HashSet<IOnlinePlayer> players = [];

  public void AddPlayer(IOnlinePlayer player) {
    players.Add(player);
    bus.Dispatch(new PlayerJoinEvent(player));
  }

  public void RemovePlayer(IPlayer player) {
    players.RemoveWhere(p => p.Id == player.Id);
    bus.Dispatch(new PlayerLeaveEvent(player));
  }

  public ISet<IOnlinePlayer> GetOnline() { return players; }
}