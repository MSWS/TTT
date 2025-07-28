using TTT.Api.Events;
using TTT.Api.Player;

namespace TTT.Game.Events.Player;

public abstract class PlayerEvent(IPlayer player) : Event {
  public IPlayer Player { get; } = player;
}