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

  private readonly List<IRole> roles = [
    new InnocentRole(), new TraitorRole(), new DetectiveRole()
  ];

  private State currentState = State.WAITING;

  public State CurrentState {
    set {
      var ev = new GameStateUpdateEvent(this, value);
      bus.Dispatch(ev);
      if (ev.IsCanceled) return;
      currentState = value;
    }

    get => currentState;
  }

  public ICollection<IPlayer> Players { get; } = new List<IPlayer>();

  public DateTime? StartedAt { get; protected set; }
  public DateTime? FinishedAt { get; protected set; } = null;

  public SortedDictionary<DateTime, ISet<IAction>> Actions {
    get;
    protected set;
  } = new();

  public void Start() {
    onlineMessenger?.BackgroundMsgAll(finder,
      "Attempting to start the game...");

    var players = finder.GetAllPlayers();

    if (players.Count < 2) {
      onlineMessenger?.BackgroundMsgAll(finder,
        "Not enough players to start the game.");
      return;
    }

    if (CurrentState != State.WAITING) {
      onlineMessenger?.BackgroundMsgAll(finder, "Game is already in progress.");
      return;
    }

    CurrentState = State.COUNTDOWN;

    onlineMessenger?.MessageAll(finder, "Game is starting in 5 seconds...");

    Task.Delay(5000)
     .ContinueWith(_ => {
        if (CurrentState != State.COUNTDOWN) return;
        startRound();
      });
  }

  private void startRound() {
    CurrentState = State.IN_PROGRESS;

    StartedAt = DateTime.Now;

    assigner.AssignRoles(finder.GetAllPlayers(), roles);
  }
}