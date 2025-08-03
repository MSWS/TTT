using TTT.API.Storage;

namespace TTT.Test.Fakes;

public class MemoryStorage<T> : IStorage<T>, IWriteable<T> where T : class {
  private T? data;

  public Task<T?> Load() {
    if (data is null)
      throw new InvalidOperationException("Data not initialized");

    return Task.FromResult<T?>(data);
  }

  public Task Write(T newData) {
    data = newData
      ?? throw new ArgumentNullException(nameof(newData),
        "Data cannot be null");
    return Task.CompletedTask;
  }

  public Task Save() {
    // In-memory storage does not need to save anything
    return Task.CompletedTask;
  }
}