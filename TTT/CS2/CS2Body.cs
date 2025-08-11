using CounterStrikeSharp.API.Core;
using TTT.API;
using TTT.API.Player;
using TTT.Game;
using TTT.Game.Roles;

namespace TTT.CS2;

public class CS2Body(CRagdollProp ragdoll, IPlayer player) : IBody {
  public CRagdollProp Ragdoll { get; } = ragdoll;
  public IPlayer OfPlayer { get; } = player;
  public bool IsIdentified { get; private set; }
  public IWeapon? MurderWeapon { get; private set; }

  public IPlayer? Killer { get; private set; }


  public CS2Body WithWeapon(IWeapon weapon) {
    MurderWeapon = weapon;
    return this;
  }

  public CS2Body WithWeapon(string weapon) {
    return WithWeapon(new BaseWeapon(weapon));
  }

  public CS2Body WithKiller(IPlayer? killer) {
    Killer = killer;
    return this;
  }

  public CS2Body Identified(bool identified = true) {
    IsIdentified = identified;
    return this;
  }
}