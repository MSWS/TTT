using TTT.Api.Events;
using TTT.Api.Player;
using TTT.Game.Events.Player;

namespace TTT.Test.Fakes;

public class FakePlayerFinder(IEventBus bus) : IPlayerFinder {
  private readonly HashSet<IOnlinePlayer> players = [];

  public void addPlayer(IOnlinePlayer player) {
    players.Add(player);
    bus.Dispatch(new PlayerJoinEvent(player));
  }

  public void removePlayer(IPlayer player) {
    players.RemoveWhere(p => p.Id == player.Id);
    bus.Dispatch(new PlayerLeaveEvent(player));
  }

  public ISet<IOnlinePlayer> GetAllPlayers() { return players; }
}