namespace TTT.API.Game;

public interface IGameManager : IDisposable {
  IGame? ActiveGame { get; protected set; }

  void IDisposable.Dispose() {
    ActiveGame?.Dispose();
    ActiveGame = null;
  }

  IGame? CreateGame();

  [Obsolete("This method is ambiguous, check the game state directly.")]
  bool IsGameActive() {
    return ActiveGame is not null && ActiveGame.IsInProgress();
  }
}