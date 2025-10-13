using System.Collections.Concurrent;
using System.Data;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Karma.Events;

namespace TTT.Karma;

public sealed class KarmaStorage(IServiceProvider provider) : IKarmaService {
  private readonly IEventBus _bus = provider.GetRequiredService<IEventBus>();

  private readonly IScheduler _scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IStorage<KarmaConfig>? _configStorage =
    provider.GetService<IStorage<KarmaConfig>>();

  // Cache keyed by stable player id to avoid relying on IPlayer equality
  private readonly ConcurrentDictionary<string, int> _karmaCache = new();

  private KarmaConfig _config = new();
  private IDbConnection? _connection;
  private IDisposable? _flushSubscription;
  private readonly SemaphoreSlim _flushGate = new(1, 1);

  // Toggle immediate writes. If false, every Write triggers a flush
  private const bool EnableCache = true;

  public string Id => nameof(KarmaStorage);
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() {
    // Load configuration first
    if (_configStorage is not null) {
      // Synchronously wait here since IKarmaService.Start is sync
      _config = _configStorage.Load().GetAwaiter().GetResult()
        ?? new KarmaConfig();
    }

    // Open a dedicated connection used only by this service
    _connection = new SqliteConnection(_config.DbString);
    _connection.Open();

    // Ensure schema before any reads or writes
    _connection.Execute(@"CREATE TABLE IF NOT EXISTS PlayerKarma (
          PlayerId TEXT PRIMARY KEY,
          Karma    INTEGER NOT NULL
        )");

    // Periodic flush with proper error handling and serialization
    _flushSubscription = Observable
     .Interval(TimeSpan.FromSeconds(30), _scheduler)
     .SelectMany(_ => FlushAsync().ToObservable())
     .Subscribe(_ => { }, // no-op on success
        ex => {
          // Replace with your logger if available
          System.Diagnostics.Trace.TraceError($"Karma flush failed: {ex}");
        });
  }

  public async Task<int> Load(IPlayer player) {
    if (player is null) throw new ArgumentNullException(nameof(player));
    var key = player.Id;

    if (EnableCache && _karmaCache.TryGetValue(key, out var cached))
      return cached;

    var conn = EnsureConnection();

    // Parameterize the default value to keep SQL static
    var sql = @"
SELECT COALESCE(
  (SELECT Karma FROM PlayerKarma WHERE PlayerId = @PlayerId),
  @DefaultKarma
)";
    var karma = await conn.QuerySingleAsync<int>(sql,
      new { PlayerId = key, DefaultKarma = _config.DefaultKarma });

    if (EnableCache) _karmaCache[key] = karma;
    return karma;
  }

  public async Task Write(IPlayer player, int newValue) {
    if (player is null) throw new ArgumentNullException(nameof(player));
    var key = player.Id;

    var max = _config.MaxKarma(player);
    if (newValue > max)
      throw new ArgumentOutOfRangeException(nameof(newValue),
        $"Karma must be less than {max} for player {key}.");

    int oldValue;
    if (!_karmaCache.TryGetValue(key, out oldValue)) {
      oldValue = await Load(player);
    }

    if (oldValue == newValue) return;

    var evt = new KarmaUpdateEvent(player, oldValue, newValue);
    try { _bus.Dispatch(evt); } catch {
      // Replace with your logger if available
      System.Diagnostics.Trace.TraceError(
        "Exception during KarmaUpdateEvent dispatch.");
      throw;
    }

    if (evt.IsCanceled) return;

    _karmaCache[key] = newValue;

    if (!EnableCache) await FlushAsync();
  }

  private async Task FlushAsync() {
    var conn = EnsureConnection();

    // Fast path if there is nothing to flush
    if (_karmaCache.IsEmpty) return;

    await _flushGate.WaitAsync().ConfigureAwait(false);
    try {
      // Snapshot to avoid long lock on dictionary and to provide a stable view
      var snapshot = _karmaCache.ToArray();
      if (snapshot.Length == 0) return;

      using var tx = conn.BeginTransaction();
      const string upsert = @"
INSERT INTO PlayerKarma (PlayerId, Karma)
VALUES (@PlayerId, @Karma)
ON CONFLICT(PlayerId) DO UPDATE SET Karma = excluded.Karma
";
      foreach (var (playerId, karma) in snapshot) {
        await conn.ExecuteAsync(upsert,
          new { PlayerId = playerId, Karma = karma }, tx);
      }

      tx.Commit();
    } finally { _flushGate.Release(); }
  }

  private IDbConnection EnsureConnection() {
    if (_connection is not { State: ConnectionState.Open })
      throw new InvalidOperationException(
        "Storage connection is not initialized.");
    return _connection;
  }

  public void Dispose() {
    try {
      _flushSubscription?.Dispose();
      // Best effort final flush
      if (_connection is { State: ConnectionState.Open }) {
        FlushAsync().GetAwaiter().GetResult();
      }
    } catch (Exception ex) {
      System.Diagnostics.Trace.TraceError($"Dispose flush failed: {ex}");
    } finally {
      _connection?.Dispose();
      _flushGate.Dispose();
    }
  }
}