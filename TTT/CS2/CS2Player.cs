using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.CS2;

/// <summary>
///   A CS2-specific implementation of <see cref="IOnlinePlayer" />.
///   Human Players will **always** be tracked by their Steam ID.
///   Non-human Players (bots) will be tracked by their entity index.
///   Note that slot numbers are not guaranteed to be stable across server restarts.
/// </summary>
public class CS2Player : IOnlinePlayer {
  protected CS2Player(string id, string name) {
    Id   = id;
    Name = name;
  }

  public CS2Player(ulong steam) {
    Id   = steam.ToString();
    Name = Player?.PlayerName ?? Id;
  }

  public CS2Player(int index) {
    Id   = index.ToString();
    Name = Player?.PlayerName ?? Id;
  }

  public CS2Player(CCSPlayerController player) {
    Id   = GetKey(player);
    Name = player.PlayerName;
  }

  private CCSPlayerController? Player {
    get {
      var player = Utilities.GetPlayerFromSteamId(ulong.Parse(Id))
        ?? Utilities.GetPlayerFromIndex(int.Parse(Id));
#if DEBUG
      if (player == null || !player.IsValid)
        Console.WriteLine("Failed to find player with ID: " + Id);
#endif
      return player is { IsValid: true } ? player : null;
    }
  }

  public string Id { get; }
  public string Name { get; }

  public ICollection<IRole> Roles { get; } = [];

  public int Health {
    get => Player?.Pawn.Value != null ? Player.Pawn.Value.Health : 0;

    set {
      if (Player?.Pawn.Value == null) return;
      Player.Pawn.Value.Health = value;
      Utilities.SetStateChanged(Player.Pawn.Value, "CBaseEntity", "m_iHealth");
    }
  }

  public int MaxHealth {
    get => Player?.Pawn.Value == null ? 0 : Player.Pawn.Value.MaxHealth;

    set {
      if (Player?.Pawn.Value == null) return;
      Player.Pawn.Value.MaxHealth = value;
      Utilities.SetStateChanged(Player.Pawn.Value, "CBaseEntity",
        "m_iMaxHealth");
    }
  }

  public int Armor {
    get
      => Player?.PlayerPawn.Value != null ?
        Player.PlayerPawn.Value.ArmorValue :
        0;

    set {
      if (Player?.PlayerPawn.Value == null) return;
      Player.PlayerPawn.Value.ArmorValue = value;
      Utilities.SetStateChanged(Player.PlayerPawn.Value, "CCSPlayerPawn",
        "m_ArmorValue");
    }
  }

  public bool IsAlive {
    get => Player != null && Player.PawnIsAlive;

    set
      => throw new NotSupportedException(
        "Setting IsAlive is not supported in CS2.");
  }

  public static string GetKey(CCSPlayerController player) {
    if (player.IsBot || player.IsHLTV) return player.Index.ToString();
    return player.SteamID.ToString();
  }
}