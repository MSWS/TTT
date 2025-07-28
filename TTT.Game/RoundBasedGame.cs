using TTT.Api;
using TTT.Api.Events;
using TTT.Api.Player;
using TTT.Game.Events.Game;

namespace TTT.Game;

public class RoundBasedGame(IEventBus bus) : IGame {
  public ICollection<IPlayer> Players { get; } = new List<IPlayer>();

  public DateTime? StartedAt { get; } = null;
  public DateTime? FinishedAt { get; } = null;
  public SortedDictionary<DateTime, ISet<IAction>> Actions { get; } = new();

  public void Start() {
    
  }

  public enum State {
    /// <summary>
    /// Waiting for players to join.
    /// </summary>
    WAITING,

    /// <summary>
    /// Waiting for the countdown to finish before starting the game.
    /// </summary>
    COUNTDOWN,

    /// <summary>
    /// Currently playing the game.
    /// </summary>
    IN_PROGRESS,

    /// <summary>
    /// Game has finished.
    /// </summary>
    FINISHED
  }

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
}