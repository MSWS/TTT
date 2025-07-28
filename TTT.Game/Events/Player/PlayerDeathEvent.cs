using CounterStrikeSharp.API.Core;
using TTT.Api;
using TTT.Api.Player;

namespace TTT.Game.Events.Player;

public class PlayerDeathEvent(IPlayer player) : PlayerEvent(player) {
  public PlayerDeathEvent(IPlayerConverter<CCSPlayerController> converter,
    EventPlayerDeath ev) : this(converter.GetPlayer(ev.Userid!)) {
    if (ev.Assister != null) Assister = converter.GetPlayer(ev.Assister);
    if (ev.Attacker != null) Killer   = converter.GetPlayer(ev.Attacker);

    Headshot  = ev.Headshot;
    NoScope   = ev.Noscope;
    ThruSmoke = ev.Thrusmoke;
    Weapon    = ev.Weapon;
  }

  public override string Id => "basegame.event.player.death";

  public IPlayer? Assister { get; private set; }
  public IPlayer? Killer { get; private set; }
  public bool Headshot { get; private set; }
  public bool NoScope { get; private set; }
  public bool ThruSmoke { get; private set; }
  public string Weapon { get; private set; } = string.Empty;

  public PlayerDeathEvent WithAssister(IPlayer? assister) {
    Assister = assister;
    return this;
  }

  public PlayerDeathEvent WithKiller(IPlayer? killer) {
    Killer = killer;
    return this;
  }

  public PlayerDeathEvent WithHeadshot(bool headshot = true) {
    Headshot = headshot;
    return this;
  }

  public PlayerDeathEvent WithNoScope(bool noScope = true) {
    NoScope = noScope;
    return this;
  }

  public PlayerDeathEvent WithThruSmoke(bool thruSmoke = true) {
    ThruSmoke = thruSmoke;
    return this;
  }

  public PlayerDeathEvent WithWeapon(string weapon) {
    Weapon = weapon;
    return this;
  }
}