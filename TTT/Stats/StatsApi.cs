using TTT.API.Role;
using TTT.Game.Roles;

namespace Stats;

public class StatsApi {
  public static string? API_URL
    => Environment.GetEnvironmentVariable("TTT_STATS_API_URL");

  public static string ApiNameForRole(IRole role) {
    return role switch {
      TraitorRole   => "traitor",
      DetectiveRole => "detective",
      InnocentRole  => "innocent",
      _             => "unknown"
    };
  }
}