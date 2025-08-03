using TTT.API.Storage;

namespace TTT.Test.Fakes;

public class MemoryStorage<T> : IStorage<T>, IWriteable<T> where T : class {
  private T? data;

  public Task<T?> Load() { return Task.FromResult(data); }

  public Task Write(T newData) {
    data = newData;
    return Task.CompletedTask;
  }

  public Task Save() {
    // In-memory storage does not need to save anything
    return Task.CompletedTask;
  }
}