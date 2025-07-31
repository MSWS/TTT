namespace TTT.API.Storage;

public interface
  IKeyedStorage<TKey, TValue> : IStorage<Dictionary<TKey, TValue>>
  where TKey : notnull { }