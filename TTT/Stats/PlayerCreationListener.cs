using System.Text;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace Stats;

public class PlayerCreationListener(IServiceProvider provider) : IListener {
  public void Dispose() {
    provider.GetRequiredService<IEventBus>().UnregisterListener(this);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnJoin(PlayerJoinEvent ev) {
    Task.Run(async () => await putPlayer(ev.Player));
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnLeave(PlayerLeaveEvent ev) {
    Task.Run(async () => await putPlayer(ev.Player));
  }

  private async Task putPlayer(IPlayer player) {
    var client   = provider.GetRequiredService<HttpClient>();
    var userJson = new { name = player.Name };

    var content = new StringContent(JsonSerializer.Serialize(userJson),
      Encoding.UTF8, "application/json");

    var response = await client.PutAsync("user/" + player.Id, content);
    response.EnsureSuccessStatusCode();
  }
}