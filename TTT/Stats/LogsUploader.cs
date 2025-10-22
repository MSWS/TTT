using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Events.Game;

namespace Stats;

public class LogsUploader(IServiceProvider provider) : IListener {
  private readonly HttpClient client =
    provider.GetRequiredService<HttpClient>();

  private readonly IRoundTracker roundTracker =
    provider.GetRequiredService<IRoundTracker>();

  public void Dispose() { }

  [UsedImplicitly]
  [EventHandler(Priority = Priority.MONITOR)]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    var logs = string.Join('\n', ev.Game.Logger.MakeLogs());

    var data = new { logs };

    var payload = new StringContent(
      System.Text.Json.JsonSerializer.Serialize(data),
      System.Text.Encoding.UTF8, "application/json");

    Task.Run(async () => {
      await client.PatchAsync("round/" + roundTracker.CurrentRoundId, payload);
    });
  }
}