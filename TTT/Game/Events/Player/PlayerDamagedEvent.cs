using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using TTT.API.Events;
using TTT.API.Player;

namespace TTT.Game.Events.Player;

public class PlayerDamagedEvent(IOnlinePlayer player, IOnlinePlayer? attacker,
  int dmgDealt, int hpLeft) : PlayerEvent(player), ICancelableEvent {
  private int _hpLeft = hpLeft;

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
    ArmorRemaining = ev.Userid.PawnArmor;
  }

  public PlayerDamagedEvent(IPlayerConverter<CCSPlayerController> converter,
    DynamicHook hook) : this(null!, null, 0, 0) {
    var playerPawn = hook.GetParam<CCSPlayerPawn>(0);
    var info       = hook.GetParam<CTakeDamageInfo>(1);

    var player = playerPawn.Controller.Value?.As<CCSPlayerController>();
    if (player == null || !player.IsValid)
      throw new InvalidOperationException("Player is null or invalid.");

    var attackerPawn = info.Attacker.Value?.As<CCSPlayerPawn>();
    var attacker     = attackerPawn?.OriginalController.Value;

    Weapon = info.Ability.Value?.DesignerName;
    Player = converter.GetPlayer(player) as IOnlinePlayer
      ?? throw new InvalidOperationException("Could not convert player.");
    Attacker = attacker == null || !attacker.IsValid ?
      null :
      converter.GetPlayer(attacker) as IOnlinePlayer;
    HpLeft = player.Health - DmgDealt;
  }

  public override string Id => "basegame.event.player.damaged";
  public IOnlinePlayer? Attacker { get; private set; } = attacker;

  public int ArmorDamage { get; private set; }
  public int ArmorRemaining { get; set; }
  public int DmgDealt => player.Health - HpLeft;

  public int HpLeft { get; set; }

  public string? Weapon { get; init; }
  public bool IsCanceled { get; set; }
}