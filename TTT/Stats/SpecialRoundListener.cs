using System.Text;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SpecialRound.Events;
using SpecialRoundAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Events.Game;
using TTT.Game.Listeners;

namespace Stats;

public class SpecialRoundListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IRoundTracker tracker =
    provider.GetRequiredService<IRoundTracker>();

  private readonly HttpClient client =
    provider.GetRequiredService<HttpClient>();

  private AbstractSpecialRound? round;

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundStart(SpecialRoundStartEvent ev) { round = ev.Round; }

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    if (round == null) return;

    var data = new { special_round = round.Name };
    var payload = new StringContent(JsonSerializer.Serialize(data),
      Encoding.UTF8, "application/json");

    Task.Run(async () => {
      var id = tracker.CurrentRoundId;
      if (id == null) return;
      await client.PatchAsync("round/" + id.Value, payload);
    });
  }
}