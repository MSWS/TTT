using TTT.API.Player;
using TTT.Karma;
using Xunit.Internal;

namespace TTT.Test.Fakes;

public class MemoryKarmaStorage : KeyedMemoryStorage<IPlayer, int>,
  IKarmaService {
  public void Dispose() { }
  public string Name => nameof(MemoryKarmaStorage);
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  private readonly KarmaConfig config = new();

  public override Task<int> Load(IPlayer key) {
    return Task.FromResult(data.AddOrGet(key, () => config.DefaultKarma));
  }
}