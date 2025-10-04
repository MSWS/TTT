using CounterStrikeSharp.API.Core;
using TTT.API;
using TTT.API.Player;
using TTT.Game;

namespace TTT.CS2;

public class CS2Body(CRagdollProp ragdoll, IPlayer player) : IBody {
  public CRagdollProp Ragdoll { get; } = ragdoll;
  public IPlayer OfPlayer { get; } = player;
  public bool IsIdentified { get; set; }
  public IWeapon? MurderWeapon { get; private set; }

  public IPlayer? Killer { get; set; }
  public string Id { get; } = ragdoll.Index.ToString();
  public DateTime TimeOfDeath { get; } = DateTime.Now;

  public CS2Body WithWeapon(IWeapon weapon) {
    MurderWeapon = weapon;
    return this;
  }

  public CS2Body WithKiller(IPlayer? killer) {
    Killer = killer;
    return this;
  }
}