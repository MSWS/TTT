using SpecialRoundAPI;

namespace SpecialRound.Events;

public class SpecialRoundStartEvent(AbstractSpecialRound round)
  : SpecialRoundEvent(round) { }