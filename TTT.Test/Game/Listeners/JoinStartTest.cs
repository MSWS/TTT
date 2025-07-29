using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using TTT.Api;
using TTT.Api.Events;
using TTT.Api.Messages;
using TTT.Api.Player;
using TTT.Game;
using TTT.Game.Listeners;
using Xunit;

namespace TTT.Test.Game.Listeners;

public class JoinStartTest(IServiceProvider provider) {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IOnlineMessenger messenger =
    provider.GetRequiredService<IOnlineMessenger>();

  private readonly TestScheduler scheduler =
    provider.GetRequiredService<TestScheduler>();

  [Fact]
  public void OnJoin_StartsGame_WhenTwoPlayersJoin() {
    var listener =
      new PlayerJoinGameStartListener(bus, finder, messenger, games);
    listener.Start();

    finder.addPlayer(TestPlayer.Random());
    finder.addPlayer(TestPlayer.Random());

    Assert.NotNull(games.ActiveGame);
    Assert.Equal(State.COUNTDOWN, games.ActiveGame?.State);

    scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

    Assert.Equal(State.IN_PROGRESS, games.ActiveGame?.State);
  }

  [Fact]
  public void OnJoin_ShouldPrintStarting_OnJoin() {
    var listener =
      new PlayerJoinGameStartListener(bus, finder, messenger, games);
    listener.Start();

    var player1 = TestPlayer.Random();
    var player2 = TestPlayer.Random();
    finder.addPlayer(player1);
    finder.addPlayer(player2);

    Assert.NotEmpty(player1.Messages);
    Assert.NotEmpty(player2.Messages);
  }
}