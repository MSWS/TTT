using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Karma.Events;

namespace TTT.Karma;

public sealed class KarmaStorage(IServiceProvider provider) : IKarmaService {
  private readonly HttpClient client =
    provider.GetRequiredService<HttpClient>();

  private KarmaConfig config
    => provider.GetService<IStorage<KarmaConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new KarmaConfig();

  public async Task<int> Load(IPlayer key) {
    var result = await client.GetAsync("user/" + key.Id);

    if (!result.IsSuccessStatusCode) return config.DefaultKarma;

    var content = await result.Content.ReadAsStringAsync();
    var json    = JsonDocument.Parse(content);
    if (!json.RootElement.TryGetProperty("karma", out var karmaElement))
      return config.DefaultKarma;

    return karmaElement.GetInt32();
  }

  public async Task Write(IPlayer key, int newData) {
    var oldKarma         = await Load(key);
    var karmaUpdateEvent = new KarmaUpdateEvent(key, oldKarma, newData);
    provider.GetService<IEventBus>()?.Dispatch(karmaUpdateEvent);
    if (karmaUpdateEvent.IsCanceled) return;

    var data = new { steam_id = key.Id, karma = karmaUpdateEvent.Karma };
    var payload = new StringContent(JsonSerializer.Serialize(data),
      Encoding.UTF8, "application/json");

    await client.PatchAsync("user/" + key.Id, payload);
  }

  public void Dispose() { }
  public void Start() { }
}