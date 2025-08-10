using CounterStrikeSharp.API.Core;

namespace TTT.CS2.GameHandlers;

public readonly struct MovementInfo {
  public required CBaseEntity Ragdoll { get; init; }
  public required double Distance { get; init; }
}