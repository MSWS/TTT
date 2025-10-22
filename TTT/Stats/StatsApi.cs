using TTT.API.Role;
using TTT.Game.Roles;

namespace Stats;

public class StatsApi {
  public static string? API_URL
    => Environment.GetEnvironmentVariable("TTT_STATS_API_URL");

  public static string ApiNameForRole(IRole role) {
    return role switch {
      _ when role is TraitorRole   => "traitor",
      _ when role is DetectiveRole => "detective",
      _ when role is InnocentRole  => "innocent",
      _                            => "unknown"
    };
  }
}