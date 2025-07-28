using Microsoft.Extensions.DependencyInjection;
using TTT.Api;
using TTT.Api.Events;
using TTT.Api.Messages;
using TTT.Api.Player;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.Game;

public class RoundBasedGame(IServiceProvider provider) : IGame {
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

  public DateTime? StartedAt { get; } = null;
  public DateTime? FinishedAt { get; } = null;
  public SortedDictionary<DateTime, ISet<IAction>> Actions { get; } = new();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IOnlineMessenger? onlineMessenger =
    provider.GetService<IOnlineMessenger>();

  private List<IRole> roles = [
    new InnocentRole(), new TraitorRole(), new DetectiveRole()
  ];

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
    assignRoles();
  }

  private void assignRoles() {
    var roleAssigned = false;
    var shuffledPlayers =
      finder.GetAllPlayers().OrderBy(_ => Guid.NewGuid()).ToHashSet();

    do {
      foreach (var role in roles) {
        var player = role.FindPlayerToAssign(shuffledPlayers);
        if (player is null) continue;

        var ev = new PlayerRoleAssignEvent(player, role);
        bus.Dispatch(ev);

        player.Roles.Add(ev.Role);
        roleAssigned = true;

        onlineMessenger?.BackgroundMsgAll(finder,
          $"{player.Name} has been assigned the role of {role.Name}.");
      }
    } while (roleAssigned);
  }
}