namespace TTT.API.Game;

public interface IGameManager : IDisposable {
  IGame? ActiveGame { get; protected set; }
  IGame? CreateGame();

  bool IsGameActive() {
    return ActiveGame is not null && ActiveGame.IsInProgress();
  }

  void IDisposable.Dispose() {
    ActiveGame?.Dispose();
    ActiveGame = null;
  }
}