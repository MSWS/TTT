using SpecialRoundAPI;
using TTT.API.Events;

namespace SpecialRound.Events;

public class SpecialRoundStartEvent(AbstractSpecialRound round)
  : AbstractSpecialRoundEvent(round), ICancelableEvent {
  public bool IsCanceled { get; set; }
}