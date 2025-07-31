using TTT.API.Storage;
using TTT.Game;

namespace TTT.Test.Fakes;

public class FakeConfig : IStorage<GameConfig> {
  public Task<GameConfig> Load() { return Task.FromResult(new GameConfig()); }
}