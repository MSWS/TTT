using CounterStrikeSharp.API.Core;
using TTT.Api.Player;

namespace TTT.Api.Events.Player;

public class PlayerDeathEvent(IPlayer player) : PlayerEvent(player) {
  public override string Id => "core.event.player.death";

  public IPlayer? Assister { get; private set; } = null;
  public IPlayer? Killer { get; private set; } = null;
  public bool Headshot { get; private set; } = false;
  public bool NoScope { get; private set; } = false;
  public bool ThruSmoke { get; private set; } = false;
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

  public PlayerDeathEvent(IPlayerConverter<CCSPlayerController> converter,
    EventPlayerDeath ev) : this(converter.GetPlayer(ev.Userid!)) {
    if (ev.Assister != null) Assister = converter.GetPlayer(ev.Assister);
    if (ev.Attacker != null) Killer   = converter.GetPlayer(ev.Attacker);

    Headshot  = ev.Headshot;
    NoScope   = ev.Noscope;
    ThruSmoke = ev.Thrusmoke;
    Weapon    = ev.Weapon;
  }
}