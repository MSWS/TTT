using TTT.API.Events;
using TTT.API.Player;

namespace TTT.Game.Events.Player;

public abstract class PlayerEvent(IPlayer player) : Event {
  public IPlayer Player { get; } = player;
}