using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTT.API.Events;
using TTT.Game.Events.Player;

namespace Stats;

public class PlayerCreationListener(IServiceProvider provider) : IListener {
  public void Dispose() {
    provider.GetRequiredService<IEventBus>().UnregisterListener(this);
  }

  private readonly ILogger logger = provider.GetRequiredService<ILogger>();

  [UsedImplicitly]
  [EventHandler]
  public void OnJoin(PlayerJoinEvent ev) {
    logger.LogInformation($"Player joined: {ev.Player.Name} ({ev.Player.Id})");
    if (StatsApi.API_URL == null) {
      Dispose();
      return;
    }

    logger.LogInformation("Sending player data to Stats API at {ApiUrl}",
      StatsApi.API_URL);

    Task.Run(async () => {
      // Create PUT request to /players/{steamid64}
      var url      = $"{StatsApi.API_URL}/user";
      var client   = new HttpClient();
      var userJson = new { steam_id = ev.Player.Id, name = ev.Player.Name };

      var content = new StringContent(
        System.Text.Json.JsonSerializer.Serialize(userJson),
        System.Text.Encoding.UTF8, "application/json");

      var response = await client.PutAsync(url, content);
      logger.LogInformation("Stats API response status: {StatusCode}",
        response.StatusCode);
      response.EnsureSuccessStatusCode();
    });
  }
}