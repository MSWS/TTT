using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;
using Xunit;

namespace TTT.Test.Game.Listeners;

public class RoundEndingTest {
  private readonly IEventBus bus;
  private readonly IPlayerFinder finder;
  private readonly IGame game;
  private readonly IServiceProvider provider;

  public RoundEndingTest(IServiceProvider provider) {
    this.provider = provider;
    var manager = provider.GetRequiredService<IGameManager>();
    game   = manager.CreateGame() ?? throw new InvalidOperationException();
    finder = provider.GetRequiredService<IPlayerFinder>();
    bus    = provider.GetRequiredService<IEventBus>();
  }

  [Fact]
  public void Round_EndsWhen_PlayerDies() {
    bus.RegisterListener(new PlayerCausesEndListener(provider));

    var dier = finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());
    game.Start();

    dier.IsAlive = false;
    var ev = new PlayerDeathEvent(dier);
    bus.Dispatch(ev);

    Assert.Equal(State.FINISHED, game.State);
  }

  [Fact]
  public void Round_EndsWhen_PlayerLeavesAndDies() {
    bus.RegisterListener(new PlayerCausesEndListener(provider));
    bus.RegisterListener(new PlayerDiesOnLeaveListener(provider));

    var leaver = finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());
    game.Start();

    var ev = new PlayerLeaveEvent(leaver);
    bus.Dispatch(ev);

    Assert.Equal(State.FINISHED, game.State);
  }

  [Fact]
  public void Round_EndsWhen_InnosWin() {
    bus.RegisterListener(new PlayerCausesEndListener(provider));

    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());
    game.Start();

    var traitor = game.GetAlive(typeof(TraitorRole)).First();

    traitor.IsAlive = false;
    bus.Dispatch(new PlayerDeathEvent(traitor));

    Assert.Equal(State.FINISHED, game.State);
    Assert.Equal(new InnocentRole(provider), game.WinningRole);
  }

  [Fact]
  public void Round_EndsWhen_TraitorsWin() {
    bus.RegisterListener(new PlayerCausesEndListener(provider));

    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());
    game.Start();

    var innos = game.GetAlive(typeof(InnocentRole)).ToList();
    foreach (var innocent in innos) {
      innocent.IsAlive = false;
      bus.Dispatch(new PlayerDeathEvent(innocent));
    }

    Assert.Equal(State.FINISHED, game.State);
    Assert.Equal(new TraitorRole(provider), game.WinningRole);
  }

  [Fact]
  public void Round_ContinuesWhen_InnosLeft() {
    bus.RegisterListener(new PlayerCausesEndListener(provider));
    bus.RegisterListener(new PlayerDiesOnLeaveListener(provider));

    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());
    game.Start();

    var innos = game.GetAlive(typeof(InnocentRole)).First();

    innos.IsAlive = false;
    var ev = new PlayerDeathEvent(innos);
    bus.Dispatch(ev);

    Assert.Equal(State.IN_PROGRESS, game.State);
  }
}