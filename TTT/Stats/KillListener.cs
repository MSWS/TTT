using System.Text;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Events.Player;

namespace Stats;

public class KillListener(IServiceProvider provider) : IListener {
  private readonly HttpClient client =
    provider.GetRequiredService<HttpClient>();

  private readonly IGameManager game =
    provider.GetRequiredService<IGameManager>();

  private readonly IRoundTracker? roundTracker =
    provider.GetService<IRoundTracker>();

  public void Dispose() { }

  [UsedImplicitly]
  [EventHandler]
  public void OnPlayerKill(PlayerDeathEvent ev) {
    if (game.ActiveGame is not { State: State.IN_PROGRESS }) return;
    if (ev.Killer == null) return;

    var data = new {
      killer_steam_id = ev.Killer.Id,
      victim_steam_id = ev.Victim.Id,
      round_id        = roundTracker?.CurrentRoundId,
      weapon          = ev.Weapon
    };

    var payload = new StringContent(JsonSerializer.Serialize(data),
      Encoding.UTF8, "application/json");

    Task.Run(async () => await client.PostAsync("kill", payload));
  }
}