using TTT.API.Role;

namespace TTT.API.Game;

public record EndReason {
  public IRole? WinningTeam { get; init; }
  public string? Message { get; init; }

  public static EndReason TIMEOUT
    => new EndReason {
      WinningTeam = null, Message = "Game ended due to timeout."
    };

  public static EndReason ERROR(string err)
    => new EndReason {
      WinningTeam = null, Message = "Game ended due to error: " + err
    };
}