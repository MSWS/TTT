using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using Xunit;

namespace TTT.Test.Game.Listeners;

public class DeathListenerTest {
  private readonly IEventBus bus;
  private readonly IPlayerFinder finder;
  private readonly IGame game;
  private readonly IServiceProvider provider;

  public DeathListenerTest(IServiceProvider provider) {
    this.provider = provider;
    var manager = provider.GetRequiredService<IGameManager>();
    game   = manager.CreateGame() ?? throw new InvalidOperationException();
    finder = provider.GetRequiredService<IPlayerFinder>();
    bus    = provider.GetRequiredService<IEventBus>();
  }

  [Fact]
  public void Round_EndsWhen_PlayerDies() {
    bus.RegisterListener(new PlayerDeathListener(provider));

    var dier = finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());
    game.Start();

    dier.IsAlive = false;
    var ev = new PlayerDeathEvent(dier);
    bus.Dispatch(ev);

    Assert.Equal(State.FINISHED, game.State);
  }
}