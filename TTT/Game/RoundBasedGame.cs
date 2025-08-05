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
using TTT.Game.Loggers;
using TTT.Game.Roles;

namespace TTT.Game;

public class RoundBasedGame(IServiceProvider provider) : IGame {
  private readonly GameConfig config = provider
   .GetRequiredService<IStorage<GameConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult() ?? new GameConfig();

  private readonly List<IPlayer> players = [];

  private State state = State.WAITING;

  public virtual IList<IRole> Roles { get; } = [
    new InnocentRole(provider), new TraitorRole(provider),
    new DetectiveRole(provider)
  ];

  public ICollection<IPlayer> Players => players;

  public IActionLogger Logger { get; } = new SimpleLogger(provider
   .GetRequiredService<IScheduler>());

  public DateTime? StartedAt { get; protected set; }
  public DateTime? FinishedAt { get; protected set; }

  public IRole? WinningRole { get; set; }

  public State State {
    set {
      var ev = new GameStateUpdateEvent(this, value);
      bus.Dispatch(ev);
      if (ev.IsCanceled) return;
      state = value;
    }

    get => state;
  }


  public IObservable<long>? Start(TimeSpan? countdown = null) {
    onlineMessenger?.ScreenMsgAll(finder, "Attempting to start the game...");

    var online = finder.GetOnline();

    if (online.Count < config.RoundCfg.MinimumPlayers) {
      onlineMessenger?.ScreenMsgAll(finder,
        "Not enough players to start the game.");
      return null;
    }

    if (State != State.WAITING) return null;

    if (countdown == null) {
      onlineMessenger?.ScreenMsgAll(finder, "Starting game without countdown.");
      StartRound();
      return Observable.Empty<long>();
    }

    onlineMessenger?.BackgroundMsgAll(finder,
      $"Game is starting in {countdown.Value.TotalSeconds} seconds...");
    State = State.COUNTDOWN;
    var timer = Observable.Timer(countdown.Value, scheduler);

    timer.Subscribe(_ => {
      if (State != State.COUNTDOWN) {
        onlineMessenger?.ScreenMsgAll(finder,
          "Game countdown was interrupted.");
        return;
      }

      StartRound();
    });

    return timer;
  }

  public void EndGame(EndReason? reason = null) {
    if (!((IGame)this).IsInProgress()) {
      Dispose();
      State = State.WAITING;
      return;
    }

    FinishedAt  = DateTime.Now;
    State       = State.FINISHED;
    WinningRole = reason?.WinningRole;

    onlineMessenger?.MessageAll(finder,
      WinningRole == null ?
        reason?.Message ?? "Game ended." :
        reason?.Message ?? $"{WinningRole.Name} won the game!");
  }

  public void Dispose() {
    players.Clear();
    Roles.Clear();
    Logger.ClearActions();
  }

  virtual protected void StartRound() {
    var online = finder.GetOnline();

    if (online.Count < 2) {
      onlineMessenger?.ScreenMsgAll(finder,
        "Not enough players to start the game.");
      State = State.WAITING;
      return;
    }

    State     = State.IN_PROGRESS;
    StartedAt = DateTime.Now;
    assigner.AssignRoles(finder.GetOnline(), Roles);
    players.AddRange(finder.GetOnline());
  }

  #region classDeps

  private readonly IRoleAssigner assigner =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IMessenger? onlineMessenger =
    provider.GetService<IMessenger>();

  #endregion
}