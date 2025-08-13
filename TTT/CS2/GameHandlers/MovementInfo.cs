using System.Diagnostics.CodeAnalysis;
using CounterStrikeSharp.API.Core;

namespace TTT.CS2.GameHandlers;

[method: SetsRequiredMembers]
public struct MovementInfo(float distance, CBaseEntity ragdoll) {
  public required CBaseEntity Ragdoll { get; init; } = ragdoll;
  public CEnvBeam? Beam { get; set; }
  public required float Distance { get; init; } = distance;
}