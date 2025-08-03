using CounterStrikeSharp.API.Core;
using TTT.API.Player;

namespace TTT.Game.Events.Player;

public class PlayerDeathEvent : PlayerEvent {
  public PlayerDeathEvent(IPlayer player) : base(player) {
    if (player is not IOnlinePlayer online) {
      throw new ArgumentException(
        "Player must be an online player to create a PlayerDeathEvent.",
        nameof(player));
    }

    if (online.IsAlive)
      throw new ArgumentException(
        "Player must be dead to create a PlayerDeathEvent.", nameof(player));
  }

  public PlayerDeathEvent(IPlayerConverter<CCSPlayerController> converter,
    EventPlayerDeath ev) : this(converter.GetPlayer(ev.Userid!)) {
    if (ev.Assister != null)
      Assister = converter.GetPlayer(ev.Assister) as IOnlinePlayer;
    if (ev.Attacker != null)
      Killer = converter.GetPlayer(ev.Attacker) as IOnlinePlayer;

    Headshot  = ev.Headshot;
    NoScope   = ev.Noscope;
    ThruSmoke = ev.Thrusmoke;
    Weapon    = ev.Weapon;
  }

  public override string Id => "basegame.event.player.death";

  public IOnlinePlayer? Victim => Player as IOnlinePlayer;
  public IOnlinePlayer? Assister { get; private set; }
  public IOnlinePlayer? Killer { get; private set; }
  public bool Headshot { get; private set; }
  public bool NoScope { get; private set; }
  public bool ThruSmoke { get; private set; }
  public string Weapon { get; private set; } = string.Empty;

  public PlayerDeathEvent WithAssister(IOnlinePlayer? assister) {
    Assister = assister;
    return this;
  }

  public PlayerDeathEvent WithKiller(IOnlinePlayer? killer) {
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