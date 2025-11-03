using SpecialRoundAPI;
using TTT.API.Events;

namespace SpecialRound.Events;

public abstract class AbstractSpecialRoundEvent(AbstractSpecialRound round)
  : Event {
  public AbstractSpecialRound Round => round;
}