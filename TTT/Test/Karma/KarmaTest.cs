using TTT.Karma;
using Xunit;

namespace TTT.Test.Karma;

public class KarmaTest(IKarmaService karma) {
  [Fact]
  public async Task BasePlayer_HasDefaultKarma_OnCreation() {
    var player      = TestPlayer.Random();
    var playerKarma = await karma.Load(player);
    Assert.Equal(50, playerKarma);
  }
}