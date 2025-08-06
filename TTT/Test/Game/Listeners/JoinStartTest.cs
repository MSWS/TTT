using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game;
using TTT.Game.Listeners;
using Xunit;

namespace TTT.Test.Game.Listeners;

public class JoinStartTest(IServiceProvider provider) {
  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly TestScheduler scheduler =
    provider.GetRequiredService<TestScheduler>();

  private readonly GameConfig config = new();

  [Fact]
  public void OnJoin_StartsGame_WhenTwoPlayersJoin() {
    var listener = new PlayerJoinStarting(provider);
    listener.Start();

    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());

    Assert.NotNull(games.ActiveGame);
    Assert.Equal(State.COUNTDOWN, games.ActiveGame?.State);

    scheduler.AdvanceBy(config.RoundCfg.CountDownDuration.Ticks);

    Assert.Equal(State.IN_PROGRESS, games.ActiveGame?.State);
  }

  [Fact]
  public void OnJoin_ShouldPrintStarting_OnJoin() {
    var listener = new PlayerJoinStarting(provider);
    listener.Start();

    var player1 = TestPlayer.Random();
    var player2 = TestPlayer.Random();
    finder.AddPlayer(player1);
    finder.AddPlayer(player2);

    Assert.NotEmpty(player1.Messages);
    Assert.NotEmpty(player2.Messages);
  }
}