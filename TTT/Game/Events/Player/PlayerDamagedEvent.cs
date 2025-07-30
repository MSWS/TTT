using CounterStrikeSharp.API.Core;
using TTT.Api.Events;
using TTT.Api.Player;

namespace TTT.Game.Events.Player;

public class PlayerDamagedEvent(IOnlinePlayer player, IOnlinePlayer? attacker,
  int dmgDealt, int hpLeft) : PlayerEvent(player), ICancelableEvent {
  public PlayerDamagedEvent(IPlayerConverter<CCSPlayerController> converter,
    EventPlayerHurt ev) : this(
    converter.GetPlayer(ev.Userid!) as IOnlinePlayer
    ?? throw new InvalidOperationException(),
    ev.Attacker == null ?
      null :
      converter.GetPlayer(ev.Attacker) as IOnlinePlayer, ev.DmgHealth,
    ev.Health) {
    ArmorDamage    = ev.DmgArmor;
    ArmorRemaining = ev.Armor;
    DmgDealt       = ev.DmgHealth;
    HpLeft         = ev.Health;
    Weapon         = ev.Weapon;
  }

  public override string Id => "basegame.event.player.damaged";
  public IOnlinePlayer? Attacker { get; private set; } = attacker;

  public int ArmorDamage { get; private set; }
  public int ArmorRemaining { get; private set; }
  public int DmgDealt { get; private set; } = dmgDealt;
  public int HpLeft { get; private set; } = hpLeft;
  public string Weapon { get; private set; } = string.Empty;
  public bool IsCanceled { get; set; } = false;

  public PlayerDamagedEvent WithAttacker(IOnlinePlayer? attacker) {
    Attacker = attacker;
    return this;
  }

  public PlayerDamagedEvent WithArmorDamage(int armorDamage) {
    ArmorDamage = armorDamage;
    return this;
  }

  public PlayerDamagedEvent WithArmorRemaining(int armorRemaining) {
    ArmorRemaining = armorRemaining;
    return this;
  }
}