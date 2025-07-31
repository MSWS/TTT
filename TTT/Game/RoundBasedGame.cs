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
using TTT.Locale;

namespace TTT.Game;

public class RoundBasedGame : IGame {
  private readonly IRoleAssigner assigner;

  private readonly IEventBus bus;

  private readonly IPlayerFinder finder;

  private readonly IOnlineMessenger? onlineMessenger;

  private readonly IMsgLocalizer? localizer;

  private readonly List<IPlayer> players = [];

  private readonly IServiceProvider provider;

  public RoundBasedGame(IServiceProvider provider) {
    this.provider   = provider;
    assigner        = provider.GetRequiredService<IRoleAssigner>();
    bus             = provider.GetRequiredService<IEventBus>();
    finder          = provider.GetRequiredService<IPlayerFinder>();
    scheduler       = provider.GetRequiredService<IScheduler>();
    onlineMessenger = provider.GetService<IOnlineMessenger>();
    localizer       = provider.GetService<IMsgLocalizer>();
    roles = [
      new InnocentRole(localizer), new TraitorRole(localizer),
      new DetectiveRole(localizer)
    ];
  }

  private readonly List<IRole> roles;

  private readonly IScheduler scheduler;

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