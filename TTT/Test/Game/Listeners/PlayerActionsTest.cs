using TTT.Game.Actions;
using TTT.Game.Events.Player;
using TTT.Game.Listeners.Loggers;
using Xunit;

namespace TTT.Test.Game.Listeners;

public class PlayerActionsTest(IServiceProvider provider) : GameTest(provider) {
  [Fact]
  public void Player_Kill_ShouldBeLogged() {
    Bus.RegisterListener(new PlayerActionsLogger(Provider));

    var (alice, bob, game) = CreateActiveGame();

    alice.IsAlive = false;
    var ev = new PlayerDeathEvent(alice).WithKiller(bob);
    Bus.Dispatch(ev);

    Assert.Contains(game.Logger.GetActions().Select(p => p.Item2),
      action => action is DeathAction);
  }

  [Fact]
  public void Player_Damage_ShouldBeLogged() {
    Bus.RegisterListener(new PlayerActionsLogger(Provider));

    var (alice, bob, game) = CreateActiveGame();

    var ev = new PlayerDamagedEvent(alice, bob, 10, 90);
    Bus.Dispatch(ev);

    Assert.Contains(game.Logger.GetActions().Select(p => p.Item2),
      action => action is DamagedAction);
  }

  [Fact]
  public void Player_RoleAssignment_ShouldBeLogged() {
    Bus.RegisterListener(new PlayerActionsLogger(Provider));

    var (_, _, game) = CreateActiveGame();

    Assert.Equal(2,
      game.Logger.GetActions()
       .Select(p => p.Item2)
       .Count(action => action is RoleAssignedAction));
  }
}