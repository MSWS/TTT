using TTT.API.Storage;

namespace TTT.Test.Fakes;

public class KeyedMemoryStorage<K, V> : IKeyedStorage<K, V?>, IKeyWritable<K, V>
  where K : class {
  private readonly Dictionary<K, V?> data = new();

  public Task<V?> Load(K key) {
    data.TryGetValue(key, out var value);
    return Task.FromResult(value);
  }

  public Task Write(K key, V value) {
    data[key] = value;
    return Task.CompletedTask;
  }
}