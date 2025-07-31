using TTT.API.Storage;
using TTT.Game;

namespace TTT.Test.Fakes;

public class MemoryStorage<T> : IStorage<T>, IWriteable<T> where T : class {
  private T? data;

  public Task<T> Load() {
    if (data is null) {
      throw new InvalidOperationException("Data not initialized");
    }

    return Task.FromResult(data);
  }

  public Task Save() {
    // In-memory storage does not need to save anything
    return Task.CompletedTask;
  }

  public Task Write(T newData) {
    this.data = newData
      ?? throw new ArgumentNullException(nameof(newData),
        "Data cannot be null");
    return Task.CompletedTask;
  }
}