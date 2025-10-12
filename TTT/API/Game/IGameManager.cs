namespace TTT.API.Game;

public interface IGameManager : IDisposable {
  IGame? ActiveGame { get; protected set; }

  void IDisposable.Dispose() {
    ActiveGame?.Dispose();
    ActiveGame = null;
  }

  IGame? CreateGame();
}