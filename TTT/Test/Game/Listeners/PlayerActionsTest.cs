using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Actions;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using Xunit;

namespace TTT.Test.Game.Listeners;

public class PlayerActionsTest(IServiceProvider provider) {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private (IOnlinePlayer alice, IOnlinePlayer bob, IGame game) createActiveGame(
    bool start = true) {
    var alice = TestPlayer.Random();
    var bob   = TestPlayer.Random();

    finder.AddPlayer(alice);
    finder.AddPlayer(bob);

    var game = games.CreateGame();
    Assert.NotNull(game);

    if (!start) return (alice, bob, game);

    game.Start();
    return (alice, bob, game);
  }

  [Fact]
  public void Player_Kill_ShouldBeLogged() {
    bus.RegisterListener(new GamePlayerActionsListener(provider));

    var (alice, bob, game) = createActiveGame();

    alice.IsAlive = false;
    var ev = new PlayerDeathEvent(alice).WithKiller(bob);
    bus.Dispatch(ev);

    Assert.Contains(game.Logger.GetActions().Select(p => p.Item2),
      action => action is DeathAction);
  }

  [Fact]
  public void Player_Damage_ShouldBeLogged() {
    bus.RegisterListener(new GamePlayerActionsListener(provider));

    var (alice, bob, game) = createActiveGame();

    var ev = new PlayerDamagedEvent(alice, bob, 10, 90);
    bus.Dispatch(ev);

    Assert.Contains(game.Logger.GetActions().Select(p => p.Item2),
      action => action is DamagedAction);
  }

  [Fact]
  public void Player_RoleAssignment_ShouldBeLogged() {
    bus.RegisterListener(new GamePlayerActionsListener(provider));

    var (_, _, game) = createActiveGame();

    Assert.Equal(2,
      game.Logger.GetActions()
       .Select(p => p.Item2)
       .Count(action => action is RoleAssignedAction));
  }
}