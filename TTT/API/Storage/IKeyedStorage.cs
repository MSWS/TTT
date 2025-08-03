namespace TTT.API.Storage;

public interface IKeyedStorage<TKey, TValue> where TKey : notnull {
  Task<TValue?> Load(TKey key);
}