using System.Reactive.Linq;
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
    var timer = countdown == null ?
      Observable.Empty<long>() :
      Observable.Timer(countdown.Value);

    State = countdown == null ? State.IN_PROGRESS : State.COUNTDOWN;

    Server.NextWorldUpdate(() => {
      if (RoundUtil.IsWarmup()) {
        State = State.WAITING;
        return;
      }

      timer.Subscribe(_ => {
        Server.NextWorldUpdate(() => {
          if (RoundUtil.IsWarmup()) return;
          State = State.WAITING;
          // ReSharper disable once BaseMethodCallWithDefaultParameter
          base.Start();
        });
      });
    });

    return timer;
  }
}