using System.Data;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Karma.Events;

namespace TTT.Karma;

public class KarmaStorage(IServiceProvider provider) : IKarmaService {
  private static readonly bool enableCache = true;
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly KarmaConfig config =
    provider.GetService<IStorage<KarmaConfig>>()?.Load().Result
    ?? new KarmaConfig();

  private readonly IDictionary<IPlayer, int> karmaCache =
    new Dictionary<IPlayer, int>();

  private IDbConnection? connection;

  public void Start() {
    connection = new SqliteConnection(config.DbString);
    connection.Open();

    Task.Run(async () => {
      if (connection is not { State: ConnectionState.Open })
        throw new InvalidOperationException(
          "Storage connection is not initialized.");

      await connection.ExecuteAsync("CREATE TABLE IF NOT EXISTS PlayerKarma ("
        + "PlayerId TEXT PRIMARY KEY, " + "Karma INTEGER NOT NULL)");
    });

    var scheduler = provider.GetRequiredService<IScheduler>();

    Observable.Interval(TimeSpan.FromMinutes(5), scheduler)
     .Subscribe(_ => Task.Run(async () => await updateKarmas()));
  }

  public async Task<int> Load(IPlayer key) {
    if (enableCache) {
      karmaCache.TryGetValue(key, out var cachedKarma);
      if (cachedKarma != 0) return cachedKarma;
    }

    if (connection is not { State: ConnectionState.Open })
      throw new InvalidOperationException(
        "Storage connection is not initialized.");

    return await connection.QuerySingleAsync<int>(
      $"SELECT COALESCE((SELECT Karma FROM PlayerKarma WHERE PlayerId = @PlayerId), {config.DefaultKarma})",
      new { PlayerId = key.Id });
  }

  public void Dispose() { connection?.Dispose(); }

  public string Id => nameof(KarmaStorage);
  public string Version => GitVersionInformation.FullSemVer;

  public async Task Write(IPlayer key, int newData) {
    if (newData > config.MaxKarma(key))
      throw new ArgumentOutOfRangeException(nameof(newData),
        $"Karma must be less than {config.MaxKarma(key)} for player {key.Id}.");

    if (!karmaCache.TryGetValue(key, out var oldKarma)) {
      oldKarma        = await Load(key);
      karmaCache[key] = oldKarma;
    }

    if (oldKarma == newData) return;

    var karmaUpdateEvent = new KarmaUpdateEvent(key, oldKarma, newData);
    await bus.Dispatch(karmaUpdateEvent);
    if (karmaUpdateEvent.IsCanceled) return;

    karmaCache[key] = newData;

    if (!enableCache) await updateKarmas();
  }

  private async Task updateKarmas() {
    if (connection is not { State: ConnectionState.Open })
      throw new InvalidOperationException(
        "Storage connection is not initialized.");

    var tasks = new List<Task>();
    foreach (var (player, karma) in karmaCache)
      tasks.Add(connection.ExecuteAsync(
        "INSERT INTO PlayerKarma (PlayerId, Karma) VALUES (@PlayerId, @Karma) "
        + "ON CONFLICT(PlayerId) DO UPDATE SET Karma = @Karma",
        new { PlayerId = player.Id, Karma = karma }));

    await Task.WhenAll(tasks);
  }
}