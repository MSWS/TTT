using TTT.API.Storage;
using TTT.Game;

namespace TTT.Test.Fakes;

public class FakeConfig : IStorage<TTTConfig> {
  public Task<TTTConfig?> Load() {
    return Task.FromResult<TTTConfig?>(new TTTConfig());
  }
}