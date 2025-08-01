using TTT.API.Role;

namespace TTT.API.Game;

public record EndReason(string? Message, IRole? WinningRole = null) {
  public EndReason(IRole role) : this(null, role) { }

  public static EndReason TIMEOUT(IRole? defaultTeam)
    => new EndReason("Round ended due to timeout", defaultTeam);

  public static EndReason ERROR(string err) => new EndReason(err);
}