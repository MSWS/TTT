using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Game;
using TTT.Game.Roles;

namespace TTT.Game;

public class RoundBasedGame(IServiceProvider provider) : IGame {
  private readonly IRoleAssigner assigner =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IOnlineMessenger? onlineMessenger =
    provider.GetService<IOnlineMessenger>();

  private readonly List<IPlayer> players = [];

  private readonly List<IRole> roles = [
    new InnocentRole(), new TraitorRole(), new DetectiveRole()
  ];

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private State state = State.WAITING;

  public State State {
    set {
      var ev = new GameStateUpdateEvent(this, value);
      bus.Dispatch(ev);
      if (ev.IsCanceled) return;
      state = value;
    }

    get => state;
  }

  public ICollection<IPlayer> Players => players;

  public DateTime? StartedAt { get; protected set; }
  public DateTime? FinishedAt { get; protected set; } = null;

  public SortedDictionary<DateTime, ISet<IAction>> Actions {
    get;
    protected set;
  } = new();

  public IObservable<long>? Start(TimeSpan? countdown = null) {
    onlineMessenger?.BackgroundMsgAll(finder,
      "Attempting to start the game...");

    var online = finder.GetOnline();

    if (online.Count < 2) {
      onlineMessenger?.BackgroundMsgAll(finder,
        "Not enough players to start the game.");
      return null;
    }

    if (State != State.WAITING) return null;

    if (countdown == null) {
      onlineMessenger?.BackgroundMsgAll(finder,
        "Starting game without countdown.");
      startRound();
      return Observable.Empty<long>();
    }

    onlineMessenger?.BackgroundMsgAll(finder,
      $"Game is starting in {countdown.Value.TotalSeconds} seconds...");
    State = State.COUNTDOWN;
    var timer = Observable.Timer(countdown.Value, scheduler);

    timer.Subscribe(_ => {
      if (State != State.COUNTDOWN) {
        onlineMessenger?.BackgroundMsgAll(finder,
          "Game countdown was interrupted.");
        return;
      }

      startRound();
    });

    return timer;
  }

  public void EndGame(IRole? winningTeam = null) {
    if (!((IGame)this).IsInProgress()) {
      Dispose();
      State = State.WAITING;
      return;
    }

    FinishedAt = DateTime.Now;
    State      = State.FINISHED;

    if (winningTeam == null)
      onlineMessenger?.MessageAll(finder,
        "The game was canceled or ended without a winning team.");
    else
      onlineMessenger?.MessageAll(finder, $"{winningTeam.Name} won the game!");
  }

  private void startRound() {
    var online = finder.GetOnline();

    if (online.Count < 2) {
      onlineMessenger?.BackgroundMsgAll(finder,
        "Not enough players to start the game.");
      State = State.WAITING;
      return;
    }

    State     = State.IN_PROGRESS;
    StartedAt = DateTime.Now;
    assigner.AssignRoles(finder.GetOnline(), roles);
    players.AddRange(finder.GetOnline());
  }

  public void Dispose() {
    players.Clear();
    roles.Clear();
    Actions.Clear();
  }
}