using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game;
using TTT.Game.Listeners;
using Xunit;

namespace TTT.Test.Game.Listeners;

public class GameRestartingTest(IServiceProvider provider)
  : GameTest(provider) {
  private readonly TTTConfig config = provider
   .GetRequiredService<IStorage<TTTConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult() ?? new TTTConfig();

  private readonly TestScheduler scheduler =
    provider.GetRequiredService<TestScheduler>();

  [Fact]
  public void Game_Restarts_OnEnd() {
    Bus.RegisterListener(new GameRestartListener(Provider));

    var (_, _, game) = CreateActiveGame();
    game.EndGame(EndReason.ERROR("Forced End"));

    scheduler.AdvanceBy(config.RoundCfg.TimeBetweenRounds.Ticks);

    Assert.NotSame(game, Games.ActiveGame);

    game = Games.ActiveGame;
    Assert.NotNull(game);

    Assert.Equal(State.COUNTDOWN, game.State);
  }
}