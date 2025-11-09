using SpecialRoundAPI;
using TTT.API.Events;

namespace SpecialRound.Events;

public abstract class SpecialRoundEvent(AbstractSpecialRound round) : Event {
  public AbstractSpecialRound Round { get; } = round;
}