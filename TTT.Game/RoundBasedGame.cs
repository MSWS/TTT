using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using TTT.Api;
using TTT.Api.Events;
using TTT.Api.Messages;
using TTT.Api.Player;
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

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly List<IRole> roles = [
    new InnocentRole(), new TraitorRole(), new DetectiveRole()
  ];

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

  public ICollection<IPlayer> Players { get; } = new List<IPlayer>();

  public DateTime? StartedAt { get; protected set; }
  public DateTime? FinishedAt { get; protected set; } = null;

  public SortedDictionary<DateTime, ISet<IAction>> Actions {
    get;
    protected set;
  } = new();

  public IObservable<long> Start() {
    onlineMessenger?.BackgroundMsgAll(finder,
      "Attempting to start the game...");

    var players = finder.GetAllPlayers();

    if (players.Count < 2) {
      onlineMessenger?.BackgroundMsgAll(finder,
        "Not enough players to start the game.");
      return Observable.Empty<long>();
    }

    if (State != State.WAITING) {
      onlineMessenger?.BackgroundMsgAll(finder, "Game is already in progress.");
      return Observable.Empty<long>();
    }

    State = State.COUNTDOWN;
    onlineMessenger?.MessageAll(finder, "Game is starting in 5 seconds...");

    var timer = Observable.Timer(TimeSpan.FromSeconds(5), scheduler);

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

  private void startRound() {
    var players = finder.GetAllPlayers();

    if (players.Count < 2) {
      onlineMessenger?.BackgroundMsgAll(finder,
        "Not enough players to start the game.");
      State = State.WAITING;
      return;
    }

    State     = State.IN_PROGRESS;
    StartedAt = DateTime.Now;
    assigner.AssignRoles(finder.GetAllPlayers(), roles);
  }
}