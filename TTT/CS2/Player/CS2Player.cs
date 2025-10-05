using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.Player;

/// <summary>
///   A CS2-specific implementation of <see cref="IOnlinePlayer" />.
///   Human Players will **always** be tracked by their Steam ID.
///   Non-human Players (bots) will be tracked by their entity index.
///   Note that slot numbers are not guaranteed to be stable across server restarts.
/// </summary>
public class CS2Player : IOnlinePlayer, IEquatable<CS2Player> {
  private CCSPlayerController? cachePlayer;

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
      if (cachePlayer != null && cachePlayer.IsValid) return cachePlayer;
      var player = Utilities.GetPlayerFromSteamId(ulong.Parse(Id))
        ?? Utilities.GetPlayerFromIndex(int.Parse(Id));
#if DEBUG
      if (player == null || !player.IsValid)
        Console.WriteLine("Failed to find player with ID: " + Id);
#endif
      if (player != null && player.IsValid) cachePlayer = player;
      return player is { IsValid: true } ? player : null;
    }
  }

  private int namePadding
    => Math.Min(Utilities.GetPlayers().Select(p => p.PlayerName.Length).Max(),
      24);

  public bool Equals(CS2Player? other) {
    if (other is null) return false;
    return Id == other.Id;
  }

  public string Id { get; }
  public string Name { get; }

  public int Health {
    get => Player?.Pawn.Value != null ? Player.Pawn.Value.Health : 0;

    set {
      Server.NextWorldUpdate(() => {
        if (Player?.Pawn.Value == null) return;

        if (value <= 0) {
          Player.CommitSuicide(false, true);
          return;
        }

        Player.Pawn.Value.Health = value;
        Utilities.SetStateChanged(Player.Pawn.Value, "CBaseEntity",
          "m_iHealth");
      });
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
    get => Player?.PawnArmor ?? 0;

    set {
      if (Player == null) return;
      Player.PawnArmor = value;
      Utilities.SetStateChanged(Player, "CCSPlayerController", "m_iPawnArmor");
    }
  }

  public bool IsAlive {
    get => Player != null && Player.Pawn.Value is { Health: > 0 };

    set
      => throw new NotSupportedException(
        "Setting IsAlive is not supported in CS2.");
  }

  public static string GetKey(CCSPlayerController player) {
    if (player.IsBot || player.IsHLTV) return player.Index.ToString();
    return player.SteamID.ToString();
  }

  public override int GetHashCode() { return Id.GetHashCode(); }

  public override string ToString() { return createPaddedName(); }

  // Goal: Pad the name to a fixed width for better alignment in logs
  // Left-align ID, right-align name
  private string createPaddedName() {
    return CreatePaddedName(Id, Name, namePadding + 8);
  }

  public static string CreatePaddedName(string id, string name, int len) {
    var suffix = id.Length > 5 ? id[^5..] : id.PadLeft(5, '0');
    var prefix = $"({suffix})";

    var baseStr = $"{prefix} {name}";

    if (baseStr.Length == len) return baseStr;

    if (baseStr.Length < len) {
      // Pad spaces so the name ends up right-aligned
      var padding = len - (prefix.Length + name.Length);
      return prefix + new string(' ', padding + 1) + name;
    }

    // Too long, cut off from the end of the name
    var availableForName                       = len - (prefix.Length + 1);
    if (availableForName < 0) availableForName = 0;
    var trimmedName = name.Length > availableForName ?
      name[..availableForName] :
      name;
    return $"{prefix} {trimmedName}";
  }
}