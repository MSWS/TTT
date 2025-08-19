using System.Data;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using TTT.API;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Karma.Events;

namespace TTT.Karma;

public class KarmaStorage(IServiceProvider provider)
  : IKeyedStorage<IPlayer, int>, IKeyWritable<IPlayer, int>, ITerrorModule {
  private readonly KarmaConfig config =
    provider.GetService<IStorage<KarmaConfig>>()?.Load().Result
    ?? new KarmaConfig();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private IDbConnection? connection;

  private readonly IDictionary<IPlayer, int> karmaCache =
    new Dictionary<IPlayer, int>();

  public void Start() {
    connection = new MySqlConnection(config.DbString);
    var scheduler = provider.GetRequiredService<IScheduler>();

    Observable.Interval(TimeSpan.FromMinutes(1), scheduler)
     .Subscribe(_ => updateKarmas());
  }

  private async void updateKarmas() {
    if (connection is not { State: ConnectionState.Open })
      throw new InvalidOperationException(
        "Storage connection is not initialized.");

    var tasks = new List<Task>();
    foreach (var (player, karma) in karmaCache) {
      tasks.Add(connection.ExecuteAsync(
        "INSERT INTO PlayerKarma (PlayerId, Karma) VALUES (@PlayerId, @Karma) "
        + "ON DUPLICATE KEY UPDATE Karma = @Karma",
        new { PlayerId = player.Id, Karma = karma }));
    }

    await Task.WhenAll(tasks);
  }

  public Task<int> Load(IPlayer key) {
    if (connection is not { State: ConnectionState.Open })
      throw new InvalidOperationException(
        "Storage connection is not initialized.");

    return connection.QuerySingleOrDefaultAsync<int>(
      $"SELECT IFNULL(Karma, {config.DefaultKarma}) FROM PlayerKarma WHERE PlayerId = @PlayerId",
      new { PlayerId = key.Id });
  }

  public void Dispose() { }
  public string Name => nameof(KarmaStorage);
  public string Version => GitVersionInformation.FullSemVer;

  public async Task Write(IPlayer key, int newData) {
    if (newData < config.MinKarma || newData > config.MaxKarma(key))
      throw new ArgumentOutOfRangeException(nameof(newData),
        $"Karma must be between {config.MinKarma} and {config.MaxKarma(key)} for player {key.Id}.");

    if (!karmaCache.TryGetValue(key, out var oldKarma)) {
      oldKarma        = await Load(key);
      karmaCache[key] = oldKarma;
    }

    if (oldKarma == newData) return;

    var karmaUpdateEvent = new KarmaUpdateEvent(key, oldKarma, newData);
    bus.Dispatch(karmaUpdateEvent);
    if (karmaUpdateEvent.IsCanceled) return;

    karmaCache[key] = newData;

    if (connection is not { State: ConnectionState.Open })
      throw new InvalidOperationException(
        "Storage connection is not initialized.");
  }
}