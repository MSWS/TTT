namespace TTT.Game.Listeners.Stats;

public class StatsApi {
  public static string? API_URL
    => Environment.GetEnvironmentVariable("TTT_STATS_API_URL");
}