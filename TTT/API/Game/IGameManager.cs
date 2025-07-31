namespace TTT.API.Game;

public interface IGameManager {
  IGame? ActiveGame { get; }
  IGame? CreateGame();
  bool IsGameActive() => ActiveGame is not null && ActiveGame.IsInProgress();
}