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
using TTT.Game.lang;
using TTT.Game.Loggers;
using TTT.Game.Roles;
using TTT.Locale;

namespace TTT.Game;

public class RoundBasedGame(IServiceProvider provider) : IGame {
  private readonly TTTConfig config = provider
   .GetRequiredService<IStorage<TTTConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult() ?? new TTTConfig();

  protected readonly IMsgLocalizer Locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly List<IPlayer> players = [];

  protected State state = State.WAITING;

  public virtual IList<IRole> Roles { get; } = [
    new InnocentRole(provider), new TraitorRole(provider),
    new DetectiveRole(provider)
  ];

  public ICollection<IPlayer> Players => players;

  public virtual IActionLogger Logger { get; } = new SimpleLogger(provider);

  public DateTime? StartedAt { get; protected set; }
  public DateTime? FinishedAt { get; protected set; }

  public IRole? WinningRole { get; set; }

  public IRoleAssigner RoleAssigner { get; init; } = provider
   .GetRequiredService<IRoleAssigner>();

  public virtual State State {
    set {
      var ev = new GameStateUpdateEvent(this, value);
      Bus.Dispatch(ev).GetAwaiter().GetResult();
      if (ev.IsCanceled) return;
      state = value;
    }

    get => state;
  }

  public virtual IObservable<long>? Start(TimeSpan? countdown = null) {
    Messenger?.Debug("Attempting to start the game...");
    var online = finder.GetOnline();

    if (online.Count < config.RoundCfg.MinimumPlayers) {
      Messenger?.MessageAll(
        Locale[GameMsgs.NOT_ENOUGH_PLAYERS(config.RoundCfg.MinimumPlayers)]);
      return null;
    }

    if (State != State.WAITING) return null;

    if (countdown == null) {
      StartRound();
      return Observable.Empty<long>();
    }

    Messenger?.MessageAll(
      Locale[GameMsgs.GAME_STATE_STARTING(countdown.Value)]);
    State = State.COUNTDOWN;
    var timer = Observable.Timer(countdown.Value, Scheduler);

    timer.Subscribe(_ => {
      if (State != State.COUNTDOWN) {
        Messenger?.MessageAll(
          Locale[GameMsgs.GENERIC_ERROR("Game was interrupted.")]);
        return;
      }

      StartRound();
    });

    return timer;
  }

  public void EndGame(EndReason? reason = null) {
    if (State is not State.IN_PROGRESS and not State.COUNTDOWN) {
      Dispose();
      State = State.WAITING;
      return;
    }

    FinishedAt  = DateTime.Now;
    WinningRole = reason?.WinningRole;
    State       = State.FINISHED;

    Messenger?.MessageAll(WinningRole != null ?
      Locale[GameMsgs.GAME_STATE_ENDED_TEAM_WON(WinningRole)] :
      Locale[GameMsgs.GAME_STATE_ENDED_OTHER(reason?.Message ?? "Unknown")]);
  }

  public bool CheckEndConditions() {
    if (State != State.IN_PROGRESS) return false;

    var endGame = getWinningTeam(out var winningTeam);
    if (!endGame) return false;

    EndGame(winningTeam == null ?
      new EndReason("Draw") :
      new EndReason(winningTeam));
    return true;
  }

  public void Dispose() {
    State = State.FINISHED;
    players.Clear();
    Roles.Clear();
    Logger.ClearActions();
  }

  private bool getWinningTeam(out IRole? winningTeam) {
    winningTeam = null;

    var traitorRole =
      Roles.First(r => r.GetType().IsAssignableTo(typeof(TraitorRole)));
    var innocentRole =
      Roles.First(r => r.GetType().IsAssignableTo(typeof(InnocentRole)));
    var detectiveRole = Roles.First(r
      => r.GetType().IsAssignableTo(typeof(DetectiveRole)));

    var traitorsAlive    = ((IGame)this).GetAlive(typeof(TraitorRole)).Count;
    var nonTraitorsAlive = ((IGame)this).GetAlive().Count - traitorsAlive;
    var detectivesAlive  = ((IGame)this).GetAlive(typeof(DetectiveRole)).Count;

    switch (traitorsAlive) {
      case 0 when nonTraitorsAlive == 0:
        winningTeam = null;
        return true;
      case > 0 when nonTraitorsAlive == 0:
        winningTeam = traitorRole;
        return true;
      case 0 when nonTraitorsAlive > 0:
        winningTeam = nonTraitorsAlive == detectivesAlive ?
          detectiveRole :
          innocentRole;
        return true;
      default:
        winningTeam = null;
        return false;
    }
  }

  virtual protected void StartRound() {
    var online = finder.GetOnline();

    if (online.Count < config.RoundCfg.MinimumPlayers) {
      Messenger?.MessageAll(
        Locale[GameMsgs.NOT_ENOUGH_PLAYERS(config.RoundCfg.MinimumPlayers)]);
      State = State.WAITING;
      return;
    }

    StartedAt = DateTime.Now;
    RoleAssigner.AssignRoles(online, Roles);
    players.AddRange(online.Where(p
      => RoleAssigner.GetRoles(p)
       .Any(r => r is TraitorRole or DetectiveRole or InnocentRole)));

    State = State.IN_PROGRESS;

    var traitors    = ((IGame)this).GetAlive(typeof(TraitorRole)).Count;
    var nonTraitors = players.Count - traitors;
    Messenger?.MessageAll(Locale[
      GameMsgs.GAME_STATE_STARTED(traitors, nonTraitors)]);
  }

  #region classDeps

  protected readonly IEventBus Bus = provider.GetRequiredService<IEventBus>();

  protected readonly IScheduler Scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  protected readonly IMessenger? Messenger = provider.GetService<IMessenger>();

  #endregion
}