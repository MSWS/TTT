using TTT.Karma;
using Xunit;

namespace TTT.Test.Karma;

public class KarmaStorageTests(IKarmaService karma) {
  [Fact]
  public async Task BasePlayer_HasDefaultKarma_OnCreation() {
    var player      = TestPlayer.Random();
    var playerKarma = await karma.Load(player);
    Assert.Equal(50, playerKarma);
  }

  [Fact]
  public async Task KarmaStorage_Write_PersistsKarma() {
    var player = TestPlayer.Random();
    await karma.Write(player, 75);
    var playerKarma = await karma.Load(player);
    Assert.Equal(75, playerKarma);
  }
}