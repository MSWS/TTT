using CounterStrikeSharp.API.Core;

namespace TTT.CS2.Items.Station;

public class StationInfo(CPhysicsPropMultiplayer prop, int health) {
  public readonly CPhysicsPropMultiplayer Prop = prop;
  public int Health = health;
  public int HealthGiven = 0;
}