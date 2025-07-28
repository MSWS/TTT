using CounterStrikeSharp.API.Core;
using TTT.Api.Player;

namespace TTT.Api.Events.Player;

public class PlayerDamagedEvent(IOnlinePlayer player, IOnlinePlayer? attacker,
  int dmgDealt, int hpLeft) : PlayerEvent(player), ICancelableEvent {
  public override string Id => "core.event.player.damaged";
  public bool IsCanceled { get; set; } = false;
  public IOnlinePlayer? Attacker { get; private set; } = attacker;

  public int ArmorDamage { get; private set; } = 0;
  public int ArmorRemaining { get; private set; } = 0;
  public int DmgDealt { get; private set; } = dmgDealt;
  public int HpLeft { get; private set; } = hpLeft;
  public string Weapon { get; private set; } = string.Empty;

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
}