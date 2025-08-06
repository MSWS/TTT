using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API.Game;
using TTT.API.Role;
using TTT.CS2.Roles;
using TTT.Game;

namespace TTT.CS2;

public class CS2Game(IServiceProvider provider) : RoundBasedGame(provider) {
  public override IList<IRole> Roles { get; } = [
    new SpectatorRole(provider), new CS2InnocentRole(provider),
    new CS2TraitorRole(provider), new CS2DetectiveRole(provider)
  ];

  override protected void StartRound() {
    Server.NextWorldUpdate(() => {
      base.StartRound();
      var levelChange = new EventNextlevelChanged(true);
      levelChange.FireEvent(false);
    });
  }

  // Since this can be called off the main thread, we need to ensure
  // the underlying logic is executed on the main thread.
  public override IObservable<long>? Start(TimeSpan? countdown = null) {
    // To enforce game state consistency, immediately update game state,
    // and restore the old state right before we actually call the base Start method.
    var oldState = State;
    Server.NextWorldUpdate(() => {
      if (RoundUtil.IsWarmup()) return;
      State = oldState;
      base.Start(countdown);
    });
    if (State == State.WAITING)
      State = countdown == null ? State.IN_PROGRESS : State.COUNTDOWN;
    return null;
  }
}