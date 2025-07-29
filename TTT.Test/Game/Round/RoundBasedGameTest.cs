using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using TTT.Api.Player;
using TTT.Game;
using Xunit;

namespace TTT.Test.Game.Round;

public class RoundBasedGameTest(IServiceProvider provider) {
  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly TestScheduler scheduler =
    provider.GetRequiredService<TestScheduler>();

  [Fact]
  public void Start_StartsGame_WithPlayers() {
    var game = new RoundBasedGame(provider);

    finder.addPlayer(TestPlayer.Random());
    finder.addPlayer(TestPlayer.Random());

    game.Start();

    scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);
    Assert.Equal(State.IN_PROGRESS, game.State);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(1)]
  public void Start_DoesNotStart_WithoutPlayers(int players) {
    var game = new RoundBasedGame(provider);

    for (var i = 0; i < players; i++) finder.addPlayer(TestPlayer.Random());

    game.Start();

    scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

    Assert.Equal(State.WAITING, game.State);
  }

  [Fact]
  public void Start_DoesNotImmediatelyStart_WithoutDelay() {
    var game = new RoundBasedGame(provider);

    finder.addPlayer(TestPlayer.Random());
    finder.addPlayer(TestPlayer.Random());

    game.Start();

    // Advance by less than the delay time
    scheduler.AdvanceBy(TimeSpan.FromSeconds(4).Ticks);

    Assert.Equal(State.COUNTDOWN, game.State);
  }
}