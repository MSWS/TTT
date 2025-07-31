using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Game.Roles;

namespace TTT.Game;

public class RoundBasedGame(IServiceProvider provider) : IGame {
  private readonly IRoleAssigner assigner =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly GameConfig config = provider
   .GetRequiredService<IStorage<GameConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IOnlineMessenger? onlineMessenger =
    provider.GetService<IOnlineMessenger>();

  private readonly List<IPlayer> players = [];

  private readonly List<IRole> roles = [
    new InnocentRole(provider), new TraitorRole(provider),
    new DetectiveRole(provider)
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
  public DateTime? FinishedAt { get; protected set; }

  public SortedDictionary<DateTime, ISet<IAction>> Actions {
    get;
    protected set;
  } = new();

  public IObservable<long>? Start(TimeSpan? countdown = null) {
    onlineMessenger?.BackgroundMsgAll(finder,
      "Attempting to start the game...");

    var online = finder.GetOnline();

    if (online.Count < config.RoundCfg.MinimumPlayers) {
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

    onlineMessenger?.MessageAll(finder,
      winningTeam == null ?
        "The game was canceled or ended without a winning team." :
        $"{winningTeam.Name} won the game!");
  }

  public void Dispose() {
    players.Clear();
    roles.Clear();
    Actions.Clear();
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
}