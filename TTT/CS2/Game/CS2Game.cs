using System.Reactive.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API.Game;
using TTT.API.Role;
using TTT.CS2.Roles;
using TTT.CS2.Utils;
using TTT.Game;
using TTT.Game.Events.Game;
using TTT.Game.Roles;

namespace TTT.CS2.Game;

public class CS2Game(IServiceProvider provider) : RoundBasedGame(provider) {
  public override State State {
    set {
      var ev = new GameStateUpdateEvent(this, value);
      Bus.Dispatch(ev);
      if (ev.IsCanceled) return;
      state = value;
    }

    get => state;
  }

  public override IActionLogger Logger { get; } = new CS2Logger(provider);

  public override IList<IRole> Roles { get; } = [
    new SpectatorRole(provider), new InnocentRole(provider),
    new TraitorRole(provider), new DetectiveRole(provider)
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
    if (State != State.WAITING) return null;
    var timer = countdown == null ?
      Observable.Empty<long>() :
      Observable.Timer(countdown.Value);

    State = countdown == null ? State.IN_PROGRESS : State.COUNTDOWN;

    Server.NextWorldUpdate(() => {
      if (RoundUtil.IsWarmup()) {
        State = State.WAITING;
        return;
      }

      if (countdown != null)
        Messenger?.MessageAll(
          Locale[GameMsgs.GAME_STATE_STARTING(countdown.Value)]);

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