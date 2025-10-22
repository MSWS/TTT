using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners.Stats;

public class PlayerCreationListener(IServiceProvider provider) : IListener {
  public void Dispose() {
    provider.GetRequiredService<IEventBus>().UnregisterListener(this);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnJoin(PlayerJoinEvent ev) {
    if (StatsApi.API_URL == null) {
      Dispose();
      return;
    }

    Task.Run(async () => {
      // Create PUT request to /players/{steamid64}
      var url      = $"{StatsApi.API_URL}/user";
      var client   = new HttpClient();
      var userJson = new { steam_id = ev.Player.Id, name = ev.Player.Name };

      var content = new StringContent(
        System.Text.Json.JsonSerializer.Serialize(userJson),
        System.Text.Encoding.UTF8, "application/json");

      var response = await client.PutAsync(url, content);
      response.EnsureSuccessStatusCode();
    });
  }
}