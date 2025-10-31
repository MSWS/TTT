using System.Text;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Stats.lang;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.Game.Events.Game;
using TTT.Locale;

namespace Stats;

public class LogsUploader(IServiceProvider provider) : IListener {
  private readonly HttpClient client =
    provider.GetRequiredService<HttpClient>();

  private readonly IMsgLocalizer localizer =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  private readonly IRoundTracker roundTracker =
    provider.GetRequiredService<IRoundTracker>();

  public void Dispose() { }

  [UsedImplicitly]
  [EventHandler(Priority = Priority.MONITOR)]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    var logs = string.Join('\n', ev.Game.Logger.MakeLogs());

    var data = new { logs };

    var payload = new StringContent(JsonSerializer.Serialize(data),
      Encoding.UTF8, "application/json");

    Task.Run(async () => {
      var id = roundTracker.CurrentRoundId;
      if (id == null) return;
      var result = await client.PatchAsync("round/" + id.Value, payload);

      if (!result.IsSuccessStatusCode) return;

      await messenger.MessageAll(localizer[StatsMsgs.API_ROUND_END(id.Value)]);
    });
  }
}