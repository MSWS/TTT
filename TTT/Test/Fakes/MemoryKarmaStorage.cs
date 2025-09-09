using TTT.API.Player;
using TTT.Karma;

namespace TTT.Test.Fakes;

public class MemoryKarmaStorage : KeyedMemoryStorage<IPlayer, int?>,
  IKarmaService {
  public void Dispose() { }
  public string Name => nameof(MemoryKarmaStorage);
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  private readonly KarmaConfig config = new KarmaConfig();

  public Task Write(IPlayer key, int newData) {
    return base.Write(key, newData);
  }
}