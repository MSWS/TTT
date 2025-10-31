using TTT.API.Events;
using TTT.API.Player;
using TTT.Karma;
using TTT.Karma.Events;
using Xunit.Internal;

namespace TTT.Test.Fakes;

public class MemoryKarmaStorage(IEventBus bus)
  : KeyedMemoryStorage<IPlayer, int>, IKarmaService {
  private readonly KarmaConfig config = new();
  public void Dispose() { }
  public string Id => nameof(MemoryKarmaStorage);
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  public override async Task Write(IPlayer key, int value) {
    var old        = await Load(key);
    var karmaEvent = new KarmaUpdateEvent(key, old, value);
    bus.Dispatch(karmaEvent);

    if (karmaEvent.IsCanceled) return;

    await base.Write(key, karmaEvent.Karma);
  }

  public override Task<int> Load(IPlayer key) {
    return Task.FromResult(data.GetOrAdd(key, config.DefaultKarma));
  }
}