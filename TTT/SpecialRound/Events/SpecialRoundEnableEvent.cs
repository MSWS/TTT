using SpecialRoundAPI;

namespace SpecialRound.Events;

/// <summary>
/// Called when a special round is enabled.
/// Note that multiple special rounds may be enabled per round.
/// </summary>
/// <param name="round"></param>
public class SpecialRoundEnableEvent(AbstractSpecialRound round)
  : SpecialRoundEvent(round);