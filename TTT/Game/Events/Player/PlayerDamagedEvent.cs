using CounterStrikeSharp.API.Core;
using TTT.API.Events;
using TTT.API.Player;

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
    Weapon         = ev.Weapon;
  }

  public PlayerDamagedEvent(IPlayerConverter<CCSPlayerController> converter,
    EventPlayerFalldamage ev) : this(
    converter.GetPlayer(ev.Userid!) as IOnlinePlayer
    ?? throw new InvalidOperationException(), null, (int)ev.Damage,
    ev.Userid!.Health) {
    ArmorDamage    = 0;
    ArmorRemaining = ev.Userid.PawnArmor;
  }

  public override string Id => "basegame.event.player.damaged";
  public IOnlinePlayer? Attacker { get; private set; } = attacker;

  public int ArmorDamage { get; private set; }
  public int ArmorRemaining { get; set; }
  public int DmgDealt { get; private set; } = dmgDealt;

  private int _hpLeft = hpLeft;

  public int HpLeft {
    get => _hpLeft;
    set {
      if (value <= 0)
        throw new ArgumentOutOfRangeException(nameof(value),
          "HpLeft must be greater than 0.");
      _hpLeft = value;
    }
  }

  public string? Weapon { get; private set; }
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