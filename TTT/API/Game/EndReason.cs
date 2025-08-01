using TTT.API.Role;

namespace TTT.API.Game;

public record EndReason {
  public IRole? WinningRole { get; init; }
  public string? Message { get; init; }

  public EndReason(IRole role) { this.WinningRole = role; }

  public EndReason(string msg) {
    this.WinningRole = null;
    this.Message     = msg;
  }

  public static EndReason TIMEOUT()
    => new EndReason("Round ended due to timeout");

  public static EndReason ERROR(string err) => new EndReason(err);
}