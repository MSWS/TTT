using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.Karma.Events;

public class KarmaUpdateEvent(IPlayer player, int oldKarma, int newKarma)
  : PlayerEvent(player), ICancelableEvent {
  public override string Id => "karma.update";
  public bool IsCanceled { get; set; }
}