using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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
    var json    = System.Text.Json.JsonDocument.Parse(content);
    if (!json.RootElement.TryGetProperty("karma", out var karmaElement))
      return config.DefaultKarma;

    return karmaElement.GetInt32();
  }

  public Task Write(IPlayer key, int newData) {
    var data = new { steam_id = key.Id, karma = newData };

    var payload = new StringContent(
      System.Text.Json.JsonSerializer.Serialize(data),
      System.Text.Encoding.UTF8, "application/json");

    return client.PatchAsync("user/" + key.Id, payload);
  }

  public void Dispose() { }
  public void Start() { }
}