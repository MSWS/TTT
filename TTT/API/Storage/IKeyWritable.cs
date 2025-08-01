namespace TTT.API.Storage;

public interface IKeyWritable<TKey, TValue> where TKey : notnull {
  /// <summary>
  ///   Writes new data to the storage, replacing any existing data for the given key.
  /// </summary>
  /// <param name="key">The key for which to write the new data.</param>
  /// <param name="newData">The new data to write.</param>
  Task Write(TKey key, TValue newData);
}