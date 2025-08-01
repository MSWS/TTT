namespace TTT.API.Game;

public interface IGameManager {
  IGame? ActiveGame { get; }
  IGame? CreateGame();

  bool IsGameActive() {
    return ActiveGame is not null && ActiveGame.IsInProgress();
  }
}