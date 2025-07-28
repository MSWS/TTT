using TTT.Api.Player;

namespace TTT.Api.Events.Player;

public abstract class PlayerEvent(IPlayer player) : Event {
  public IPlayer Player { get; } = player;
}