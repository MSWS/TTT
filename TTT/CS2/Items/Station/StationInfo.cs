using CounterStrikeSharp.API.Core;
using TTT.API.Player;

namespace TTT.CS2.Items.Station;

public class StationInfo(CPhysicsPropMultiplayer prop, int health,
  IPlayer owner) {
  public readonly CPhysicsPropMultiplayer Prop = prop;
  public int Health = health;
  public int HealthGiven = 0;
  public IPlayer Owner = owner;
}