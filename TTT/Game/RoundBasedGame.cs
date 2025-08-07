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
using TTT.Locale;

namespace TTT.Game;

public class RoundBasedGame(IServiceProvider provider) : IGame {
  private readonly GameConfig config = provider
   .GetRequiredService<IStorage<GameConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult() ?? new GameConfig();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly List<IPlayer> players = [];

  private State state = State.WAITING;

  public virtual IList<IRole> Roles { get; } = [
    new InnocentRole(provider), new TraitorRole(provider),
    new DetectiveRole(provider)
  ];

  public ICollection<IPlayer> Players => players;

  public IActionLogger Logger { get; } = new SimpleLogger(provider);

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

  public virtual IObservable<long>? Start(TimeSpan? countdown = null) {
    onlineMessenger?.Debug("Attempting to start the game...");
    var online = finder.GetOnline();

    if (online.Count < config.RoundCfg.MinimumPlayers) {
      onlineMessenger?.MessageAll(
        locale[GameMsgs.NOT_ENOUGH_PLAYERS(config.RoundCfg.MinimumPlayers)]);
      return null;
    }

    if (State != State.WAITING) return null;

    if (countdown == null) {
      onlineMessenger?.DebugAnnounce("Starting game without countdown.");
      StartRound();
      return Observable.Empty<long>();
    }

    onlineMessenger?.MessageAll(
      locale[GameMsgs.GAME_STATE_STARTING(countdown.Value)]);
    State = State.COUNTDOWN;
    var timer = Observable.Timer(countdown.Value, Scheduler);

    timer.Subscribe(_ => {
      if (State != State.COUNTDOWN) {
        onlineMessenger?.MessageAll(
          locale[GameMsgs.GENERIC_ERROR("Game was interrupted.")]);
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
    WinningRole = reason?.WinningRole;
    State       = State.FINISHED;

    onlineMessenger?.MessageAll(WinningRole != null ?
      locale[GameMsgs.GAME_STATE_ENDED_TEAM_WON(WinningRole)] :
      locale[GameMsgs.GAME_STATE_ENDED_OTHER(reason?.Message ?? "Unknown")]);
  }

  public void Dispose() {
    State = State.WAITING;
    players.Clear();
    Roles.Clear();
    Logger.ClearActions();
  }

  virtual protected void StartRound() {
    var online = finder.GetOnline();

    if (online.Count < config.RoundCfg.MinimumPlayers) {
      onlineMessenger?.MessageAll(
        locale[GameMsgs.NOT_ENOUGH_PLAYERS(config.RoundCfg.MinimumPlayers)]);
      State = State.WAITING;
      return;
    }

    State     = State.IN_PROGRESS;
    StartedAt = DateTime.Now;
    assigner.AssignRoles(online, Roles);
    players.AddRange(online);

    var traitors    = ((IGame)this).GetAlive(typeof(TraitorRole)).Count;
    var nonTraitors = online.Count - traitors;
    onlineMessenger?.MessageAll(locale[
      GameMsgs.GAME_STATE_STARTED(traitors, nonTraitors)]);
  }

  #region classDeps

  private readonly IRoleAssigner assigner =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  protected readonly IScheduler Scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IMessenger? onlineMessenger =
    provider.GetService<IMessenger>();

  #endregion
}