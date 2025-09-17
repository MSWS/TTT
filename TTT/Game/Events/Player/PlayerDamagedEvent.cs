using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using TTT.API.Events;
using TTT.API.Player;

namespace TTT.Game.Events.Player;

public class PlayerDamagedEvent(IOnlinePlayer player, IOnlinePlayer? attacker,
  int dmgDealt, int hpLeft) : PlayerEvent(player), ICancelableEvent {
  private int _hpLeft = hpLeft;
  public bool HpModified { get; private set; } = false;

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

  public PlayerDamagedEvent(IPlayerConverter<CCSPlayerController> converter,
    DynamicHook hook) : this(null!, null, 0, 0) {
    var playerPawn = hook.GetParam<CCSPlayerPawn>(0);
    var info       = hook.GetParam<CTakeDamageInfo>(1);

    var player = playerPawn.Controller.Value?.As<CCSPlayerController>();
    if (player == null || !player.IsValid)
      throw new InvalidOperationException("Player is null or invalid.");

    var attackerPawn = info.Attacker;
    var attacker     = attackerPawn.Value?.As<CCSPlayerController>();

    Player = converter.GetPlayer(player) as IOnlinePlayer
      ?? throw new InvalidOperationException("Could not convert player.");
    Attacker = attacker == null || !attacker.IsValid ?
      null :
      converter.GetPlayer(attacker) as IOnlinePlayer;
    DmgDealt = (int)info.Damage;
    _hpLeft  = player.Health - DmgDealt;
  }

  public override string Id => "basegame.event.player.damaged";
  public IOnlinePlayer? Attacker { get; private set; } = attacker;

  public int ArmorDamage { get; private set; }
  public int ArmorRemaining { get; set; }
  public int DmgDealt { get; private set; } = dmgDealt;

  public int HpLeft {
    get => _hpLeft;
    set {
      if (value <= 0)
        throw new ArgumentOutOfRangeException(nameof(value),
          "HpLeft must be greater than 0.");
      HpModified = _hpLeft != value;
      _hpLeft    = value;
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