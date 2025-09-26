using CounterStrikeSharp.API.Core;

namespace TTT.CS2.API;

public interface IAliveSpoofer {
  ISet<CCSPlayerController> FakeAlivePlayers { get; }
  void SpoofAlive(CCSPlayerController player);
  void UnspoofAlive(CCSPlayerController player);
}